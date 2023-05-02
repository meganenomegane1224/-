using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.Audio;

public class Admin_UI : MonoBehaviour
{
    [Header("アタッチコンポーネント")]
        [SerializeField]
        private Canvas[] canvas;            //ポーズ停止の際に切り替えられるcanvasを定義
        [SerializeField]
        private GameObject PauseCanvas;     //ポーズ画面の際に切り替えるcanvasの親。これのactiveによりポーズかどうかを判定している
        [SerializeField]
        private Image[] image;              //主に設定画面等で使われる画像コンポーネントを定義
        [SerializeField]
        private Toggledate[] toggledates;   //設定画面に使われるスイッチタイプのトグルコンポーネントを定義
        [SerializeField]
        private GameObject[] SettingPanel;  //設定項目の小さなくくりを定義
        [SerializeField]
        private Slider[] slider;            //設定画面で使うスライダーを定義
        [SerializeField]
        private GameObject[] alarmCanvas;   //表示するalarmの内容を記述したパネルをアタッチする場所
        [SerializeField]
        private Canvas ControlCanvas;       //モバイルプラットフォームの時に使われる操作ＵＩを定義
        [SerializeField]
        private Canvas AlarmParent;         //アラームを表示するたまの親オブジェクトを定義

        [SerializeField]
        private Move move;                  //moveを定義
        [SerializeField]
        private Admin admin;                //adminを定義
        [SerializeField]
        private Admin_Date admin_Date;      //admin_dateを定義
        [SerializeField]
        private ChangeEquip changeEquip;    //changeEquipを定義
        [SerializeField]
        private roteCamera roteCamera;      //roteCameraを定義
        [SerializeField]
        private CinemachineVirtualCamera virtualCamera;　//virtualCameraを定義
        [SerializeField]
        private InputField aim_x_speed;     //X軸のエイム感度を入力するInputFieldを定義
        [SerializeField]
        private InputField aim_y_speed;     //Y軸のエイム感度を入力するInputFieldを定義
        [SerializeField] 
        private Light Mainlight;            //シーン内のメインライトを定義       

    [Header("その他")]
        [SerializeField]
        private AudioMixer audioMixer;      //プロジェクト内のサウンドを制御するaudioMixerを定義
        [SerializeField]
        private Image isWorkeingMobileImage;//ゲームがモバイルプラットフォームで動いているかを定義するbool
        public float MasterVolume;
        public float BGMVolume;
        public float SEVolume;
        [SerializeField]
        private Text FPS;
        public float AIM_X_speed;
        public float AIM_Y_speed; 
        public int NawFPS;
        public bool FullScreen;
        public bool shadows;
        public int ScreenResolution;
        public bool isWorkeingMobilePlatform;
        public int BGMnumber;

    private ItemsDialog itemsDialog;
    private InventoryManager inventoryManager;
    private float time;
    private CinemachinePOV cinemachinePOV;
    private bool ChangeVolumeFlag;

    //スクリプト定数
    const float CAMERA_ROTE_SPEED_MAGNIFICATION_TO_MOBILE = 1/100*2.5f;
    
    // Start is called before the first frame update
    void Start()
    {
        itemsDialog = GetComponentInChildren<ItemsDialog>();
        inventoryManager = GetComponentInChildren<InventoryManager>();
        cinemachinePOV = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        
        QualitySettings.vSyncCount = 0;
        AIM_X_speed = PlayerPrefs.GetFloat("Aim_X");
        AIM_Y_speed = PlayerPrefs.GetFloat("Aim_Y");
        ChangeFPS(PlayerPrefs.GetInt("FPS"));
        
        if(PlayerPrefs.GetInt("FullScreen") == 0)
        {
            toggledates[0].ToggleOn();
            FullScreen = true;
        }        
        if(PlayerPrefs.GetInt("Shadows") == 0)
        {
            toggledates[1].ToggleOn();
            shadows = true;
            ChangeShadowBool();
        }
        else
        {
            shadows = false;
            ChangeShadowBool();
        }
        if(PlayerPrefs.GetInt("AutoRun") == 0)
        {
            toggledates[2].ToggleOn();
        }
        BGMnumber = PlayerPrefs.GetInt("BGM");
        if(PlayerPrefs.GetInt("AutoSave")==0 )
        {
            toggledates[3].ToggleOn();
            admin_Date.AutoSave = true;
        }
        else
        {
            admin_Date.AutoSave = false;
        }

        image[BGMnumber + 10].color = new Color(0.6f,1,0.7f);
        ChangeScreenResolution(PlayerPrefs.GetInt("ScreenResolution"));
        aim_x_speed.text = AIM_X_speed.ToString();
        aim_y_speed .text = AIM_Y_speed.ToString();
        
        foreach (var item in canvas)
        {
            item.enabled = false;
        }
        PauseCanvas.SetActive(false);
        ChangeSetting(1);

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) 
		{
            ControlCanvas.enabled = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            roteCamera._angularPerPixel = new Vector2(AIM_X_speed , AIM_Y_speed)*CAMERA_ROTE_SPEED_MAGNIFICATION_TO_MOBILE;
            isWorkeingMobilePlatform = true;
		}
		else //if (Application.platform == RuntimePlatform.WindowsPlayer) 
		{
            
            ControlCanvas.enabled = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            cinemachinePOV.m_HorizontalAxis.m_MaxSpeed = AIM_X_speed;
            cinemachinePOV.m_VerticalAxis.m_MaxSpeed = AIM_Y_speed;
            isWorkeingMobilePlatform = false;
            Destroy(ControlCanvas.gameObject.GetComponent<CanvasScaler>());
            Destroy(ControlCanvas.gameObject.GetComponent<GraphicRaycaster>());
            Destroy(ControlCanvas.gameObject);
		}

        MasterVolume = PlayerPrefs.GetFloat("MVolume");
        BGMVolume = PlayerPrefs.GetFloat("BGMVolume");
        SEVolume = PlayerPrefs.GetFloat("SEVolume");
        audioMixer.SetFloat("Master" , Mathf.Log10(MasterVolume) * 20);
        audioMixer.SetFloat("BGM" , Mathf.Log10(BGMVolume) * 20);
        audioMixer.SetFloat("SE" , Mathf.Log10(MasterVolume) * 20);
        slider[0].value = MasterVolume;
        slider[1].value = BGMVolume;
        slider[2].value = SEVolume;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            GetKeyEscape(0);
        }

        if(Input.GetKeyDown(KeyCode.B))
        {
            GetKeyEscape(1);
        }

        if(Input.GetKeyDown(KeyCode.V))
        {
            GetKeyEscape(2);
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            GetKeyEscape(3);
        }

        if(Input.GetKeyDown(KeyCode.LeftAlt))
        {
            ShowCursor();
        }

        if(Input.GetKeyUp(KeyCode.LeftAlt) && PauseCanvas.activeSelf == false)
        {
            HideCursor();
        }

        time += Time.deltaTime;
        if(time > 1)
        {
            FPS.text = 1/Time.deltaTime +  "fps";
            time = 0;
        }
    }

    public void GetKeyEscape(int n)
    {
        Toggle1();

        foreach (var item in canvas)
        {
            if(item.enabled == true)
            {
                item.enabled = false;
            }
        }
        
        if(n == 0)
        {
            canvas[0].enabled = true;
        }
        else if(n != 0)
        {
            ChangeCanvas(n);
        }
        else if(PauseCanvas == false)
        {
            Toggle1();
        }
    }

    public void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ChangeCanvas(int n)
    {
        if(canvas[0].enabled == true)canvas[0].enabled = false;

        canvas[n].enabled = true;

        if(n == 1)itemsDialog.reflesh();

        if(n == 4)ChangeSetting(1);
    }

    public void BackPausePanel()
    {
        foreach (var item in canvas)
        {
            item.enabled = false;
        }
        canvas[0].enabled = true;
    }

    public void ChangeBagPanel()
    {
        if(canvas[0].enabled == true)
        {
            canvas[0].enabled = false;
        }
        canvas[1].enabled = true;
        itemsDialog.reflesh();
    }
    public void ChangeWeaponPanel()
    {
        if(canvas[0].enabled == true)
        {
            canvas[0].enabled = false;
        }
        canvas[2].enabled = true;
    }
    public void ChangeSkillTreePanel()
    {
        if(canvas[0].enabled == true)
        {
            canvas[0].enabled = false;
        }
        canvas[3].enabled = true;
    }

    public void Toggle1(){

        PauseCanvas.SetActive(!PauseCanvas.activeSelf);
        if(ChangeVolumeFlag == true)
        {
            ChangeVolumeFlag = false;
            admin_Date.SaveDateOther(9);
        }

        if (PauseCanvas.activeSelf)
        {
            Time.timeScale = 0;
            virtualCamera.enabled = false;
            
            if(isWorkeingMobilePlatform == false)return;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            move.LastAttack();
            Time.timeScale = 1;
            virtualCamera.enabled = true;

            if(isWorkeingMobilePlatform == false)return;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }        
    }

    public void Save()
    {
        admin_Date.SaveDate();
        admin_Date.SaveDateOther(0);
    }

    public void ChangeFPS(int n)
    {
        NawFPS = n;
        Application.targetFrameRate = NawFPS;
        admin_Date.SaveDateOther(3);
        if(n == 30)
        {
            image[0].color = new Color(0.6f,1,0.7f);
            image[1].color = new Color(1,1,1);
            image[2].color = new Color(1,1,1);
        }
        else if(n == 60)
        {
            image[0].color = new Color(1,1,1);
            image[1].color = new Color(0.6f,1,0.7f);
            image[2].color = new Color(1,1,1);
        }
        else if(n == 120)
        {
            image[0].color = new Color(1,1,1);
            image[1].color = new Color(1,1,1);
            image[2].color = new Color(0.6f,1,0.7f);
        }
    }
    
    public void ChangeBGM(int n)
    {
        admin.ChangeBGM(n);
        BGMnumber = n;
        admin_Date.SaveDateOther(8);
        if(n == 0)
        {
            image[10].color = new Color(0.6f,1,0.7f);
            image[11].color = new Color(1,1,1);
            image[12].color = new Color(1,1,1);
            image[13].color = new Color(1,1,1);
        }
        else if(n == 1)
        {
            image[10].color = new Color(1,1,1);
            image[11].color = new Color(0.6f,1,0.7f);
            image[12].color = new Color(1,1,1);
            image[13].color = new Color(1,1,1);
        }
        else if(n == 2)
        {
            image[10].color = new Color(1,1,1);
            image[11].color = new Color(1,1,1);
            image[12].color = new Color(0.6f,1,0.7f);
            image[13].color = new Color(1,1,1);
        }
        else if(n == 3)
        {
            image[10].color = new Color(1,1,1);
            image[11].color = new Color(1,1,1);
            image[12].color = new Color(1,1,1);
            image[13].color = new Color(0.6f,1,0.7f);
        }
    }

    public void ChangeAIMSpeed()
    {
        AIM_X_speed = float.Parse(aim_x_speed.text);
        if(AIM_X_speed <= 0)
        {
            AIM_X_speed = 1;
            aim_x_speed.text = "1";
        }
        AIM_Y_speed = float.Parse(aim_y_speed.text);
        if(AIM_Y_speed <= 0)
        {
            AIM_Y_speed = 1;
            aim_y_speed.text = "1";
        }

        if(isWorkeingMobilePlatform == true)
        {
            roteCamera._angularPerPixel = new Vector2(AIM_X_speed , AIM_Y_speed)*CAMERA_ROTE_SPEED_MAGNIFICATION_TO_MOBILE;
        }
        else
        {
            cinemachinePOV.m_HorizontalAxis.m_MaxSpeed = AIM_X_speed;
            cinemachinePOV.m_VerticalAxis.m_MaxSpeed = AIM_Y_speed;
        }
        
        admin_Date.SaveDateOther(4);
    }

    public void ChangeToggle(int n)
    {
        if(n == 0)
        {
            FullScreen = toggledates[0].Value;
            admin_Date.SaveDateOther(5);
            ChangeScreenResolution(ScreenResolution);
        }
        if(n == 1)
        {
            shadows = toggledates[1].Value;
            admin_Date.SaveDateOther(7);
            ChangeShadowBool();
        }
        if(n == 2)
        {
            var s = toggledates[2].Value ? 0:1;
            admin.ChangeAutoRun(s);
            admin_Date.SaveDateOther(10);
        }
        if(n == 3)
        {
            admin_Date.AutoSave = toggledates[3].Value; 
            admin_Date.SaveDateOther(11);
        }
    }

    public void ChangeScreenResolution(int n)
    {
        Screen.SetResolution(Screen.currentResolution.width , Screen.currentResolution.height, FullScreen);
        ScreenResolution = n;
        admin_Date.SaveDateOther(6);
        if(n == 1)
        {
            image[3].color = new Color(0.6f,1,0.7f);
            image[4].color = new Color(1,1,1);
            image[5].color = new Color(1,1,1);
        }
        if(n == 2)
        {
            image[3].color = new Color(1,1,1);
            image[4].color = new Color(0.6f,1,0.7f);
            image[5].color = new Color(1,1,1);
        }
        if(n == 3)
        {
            image[3].color = new Color(1,1,1);
            image[4].color = new Color(1,1,1);
            image[5].color = new Color(0.6f,1,0.7f);
        }
    }

    public void ChangeShadowBool()
    {
        if(shadows == true)
        {
            Mainlight.shadows = LightShadows.Soft;
        }
        else
        {
            Mainlight.shadows = LightShadows.None;
        }
    }

    public void ChangeSetting(int n)
    {
        foreach (var item in SettingPanel)
        {
            item.SetActive(false);
        }
        image[6].color = Color.white;
        image[7].color = Color.white;
        image[8].color = Color.white;
        image[9].color = Color.white;

        SettingPanel[n-1].SetActive(true);
        image[n + 5].color = new Color(0,0.3f,1);
    }

    public void ChangeWeapon()
    {
        changeEquip.ChangeWeapon();
    }

    public void GetInput(int n)
    {
        move.GetInput(n);
    }
    
    public void ChangeVolume(int n)
    {
        if(n == 0)
        {
            MasterVolume = slider[0].value;
            audioMixer.SetFloat("Master" , Mathf.Log10(MasterVolume) * 20);
        }
        else if(n == 1)
        {
            BGMVolume = slider[1].value;
            audioMixer.SetFloat("BGM" , Mathf.Log10(BGMVolume) * 20);
        }
        else if(n == 2)
        {
            SEVolume = slider[2].value;
            audioMixer.SetFloat("SE" , Mathf.Log10(MasterVolume) * 20);
        }

        ChangeVolumeFlag = true;
    }

    public void Alarm(int n)
    {
        if(n == 0)
        {
            Instantiate(alarmCanvas[0] , -Vector3.zero , Quaternion.identity , AlarmParent.transform);
        }
    }
}