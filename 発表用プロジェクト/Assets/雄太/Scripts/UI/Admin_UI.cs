using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.Audio;

public class Admin_UI : MonoBehaviour
{
    [Header("アタッチコンポーネント")]
        [SerializeField]
        private Canvas[] canvas;//設定、アイテム、スキルツリー等のキャンバスを配列化してアタッチ
        [SerializeField]
        private GameObject PauseCanvas;//ポーズ画面になるときに更新されるUIの親オブジェクトをアタッチ
        [SerializeField]
        private Image[] image;//UI内で使われている設定等によって色が変わるなどの変化が起きる画像コンポーネントをアタッチ
        [SerializeField]
        private Toggledate[] toggledates;//設定等で使うトグルスクリプトを配列化してアタッチ
        [SerializeField]
        private GameObject[] SettingPanel;//設定パネル内でさらに細かく分かれている項目をそれぞれアタッチ
        [SerializeField]
        private Slider[] slider;//設定で使うスライダーをアタッチ
        [SerializeField]
        private GameObject[] alarmCanvas;//表示するａｌａｒｍの内容をprefab化して配列内にアタッチ
        [SerializeField]
        private Canvas ControlCanvas;//スマホ操作用のＵＩを設定するキャンバスアタッチ
        [SerializeField]
        private Canvas AlarmParent;//アラームを表示する親オブジェクトをアタッチ

        [SerializeField]
        private Move move;//アタッチ
        [SerializeField]
        public Admin admin;//アタッチ
        [SerializeField]
        public Admin_Date admin_Date;//アタッチ
        [SerializeField]
        private ChangeEquip changeEquip;//アタッチ
        [SerializeField]
        private roteCamera roteCamera;//アタッチ
        [SerializeField]
        private CinemachineVirtualCamera virtualCamera;//アタッチ
        [SerializeField]
        private InputField aim_x_speed;//X軸のエイム感度を入力するUIコンポーネントをアタッチ
        [SerializeField]
        private InputField aim_y_speed;//Y軸のエイム感度を入力するUIコンポーネントをアタッチ
        [SerializeField]
        private Light Mainlight;//メインで光ってるライトをアタッチ

    [Header("その他")]
        [SerializeField]
        private AudioMixer audioMixer;//ゲーム内の音量を調整するaudioMixerをアタッチ
        public float MasterVolume;//マスター音量を定義
        public float BGMVolume;//BGm音量を定義
        public float SEVolume;//SE音量を定義
        [SerializeField]
        private Text FPS;//FPSを表示するテキストをアタッチ
        public float AIM_X_speed;//X軸のエイム感度を定義
        public float AIM_Y_speed; //Y軸のエイム感度を定義
        public int NawFPS;//ターゲットFPSを定義
        public bool FullScreen;//フルスクリーン状態かを定義
        public bool shadows;//影があるかないかを定義
        public int ScreenResolution;//解像度を定義しているらしい？　機能としては死んでいる
        public bool isWorkeingMobilePlatform;//このゲームがスマホで動いているかいないかを定義
        public int BGMnumber;//現在のBGMの番号を定義

    private ItemsDialog itemsDialog;//アタッチ
    private InventoryManager inventoryManager;//アタッチ
    private float time;//FPSを更新する時間をカウント
    private CinemachinePOV cinemachinePOV;//勝手にアタッチされる　POVを制御するコンポーネント
    private bool ChangeVolumeFlag;//音量関連の設定がポーズ中に実行されたかどうか
    public bool ChangeSkillTree = false;

    //スクリプト定数
    const float CAMERA_ROTE_SPEED_MAGNIFICATION_TO_MOBILE = 1/100*2.5f;
    
    // Start is called before the first frame update
    void Start()
    {
        itemsDialog = GetComponentInChildren<ItemsDialog>();//取得
        inventoryManager = GetComponentInChildren<InventoryManager>();//取得
        cinemachinePOV = virtualCamera.GetCinemachineComponent<CinemachinePOV>();//すでにアタッチされてるバーチャルカメラから取得
        GetComponentInChildren<Admin_SkillTree>().StartByAdmin_UI();
        
        QualitySettings.vSyncCount = 0;//FPSの自動変動を防止
        //保存されているエイム、FPS設定情報等をロード
        AIM_X_speed = PlayerPrefs.GetFloat("Aim_X");
        AIM_Y_speed = PlayerPrefs.GetFloat("Aim_Y");
        ChangeFPS(PlayerPrefs.GetInt("FPS"));
        
        if(PlayerPrefs.GetInt("FullScreen") == 0)//フルスクリーン情報をロード
        {
            toggledates[0].ToggleOn();
            FullScreen = true;
        }        
        if(PlayerPrefs.GetInt("Shadows") == 0)//影情報をロード
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
        if(PlayerPrefs.GetInt("AutoRun") == 0)//自動ダッシュ情報をロード
        {
            toggledates[2].ToggleOn();
        }
        BGMnumber = PlayerPrefs.GetInt("BGM");//BGｍ情報をロード
        if(PlayerPrefs.GetInt("AutoSave")==0 )//自動保存情報をロード
        {
            toggledates[3].ToggleOn();
            admin_Date.AutoSave = true;
        }
        else
        {
            admin_Date.AutoSave = false;
        }

        image[BGMnumber + 10].color = new Color(0.6f,1,0.7f);//再生されているBGMのボタン画像の色を変更
        ChangeScreenResolution(PlayerPrefs.GetInt("ScreenResolution"));//スクリーン情報を編集
        //エイム感度情報をUIのテキストに反映
        aim_x_speed.text = AIM_X_speed.ToString();
        aim_y_speed .text = AIM_Y_speed.ToString();
        
        foreach (var item in canvas)//それぞれのcanvasを非表示にする
        {
            item.enabled = false;
        }
        PauseCanvas.SetActive(false);//非表示にする
        ChangeSetting(1);//設定を初期画面にする

        // ゲームがモバイルで動いているか判定
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) 
		{
            //動いている場合、カーソルをロックしない、バーチャルコントローラーを表示させる
            ControlCanvas.enabled = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            roteCamera._angularPerPixel = new Vector2(AIM_X_speed , AIM_Y_speed)*CAMERA_ROTE_SPEED_MAGNIFICATION_TO_MOBILE;//感度を設定
            isWorkeingMobilePlatform = true;
		}
		else //if (Application.platform == RuntimePlatform.WindowsPlayer) 
		{
            //動いていない場合、カーソルをロックし、バーチャルコントローラーを削除する
            ControlCanvas.enabled = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.Log(AIM_X_speed);
            cinemachinePOV.m_HorizontalAxis.m_MaxSpeed = AIM_X_speed;//感度を設定
            cinemachinePOV.m_VerticalAxis.m_MaxSpeed = AIM_Y_speed;
            Debug.Log("感度更新できてるはず");
            isWorkeingMobilePlatform = false;
            Destroy(ControlCanvas.gameObject.GetComponent<CanvasScaler>());
            Destroy(ControlCanvas.gameObject.GetComponent<GraphicRaycaster>());
            Destroy(ControlCanvas.gameObject);
		}

        // 各音量をロード、設定し、設定画面に反映
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
        if(Input.GetKeyDown(KeyCode.Escape))//エスケープが押されたら、ポーズ画面にする
        {
            GetKeyEscape(0);
        }

        if(Input.GetKeyDown(KeyCode.B))//Bキーが押されたら、バッグ画面にする
        {
            GetKeyEscape(1);
        }

        if(Input.GetKeyDown(KeyCode.V))//Vキーが押されたら、何かを表示させる
        {
            GetKeyEscape(2);
        }

        if(Input.GetKeyDown(KeyCode.T))//Tキーが押されたらスキルツリーを表示する
        {
            GetKeyEscape(3);
        }

        if(Input.GetKeyDown(KeyCode.LeftAlt))//左のシフトキーが押されている間はカーソルを見せる
        {
            ShowCursor();
        }

        if(Input.GetKeyUp(KeyCode.LeftAlt) && PauseCanvas.activeSelf == false)//左のシフトキーから指が離れたらカーソルを隠す
        {
            HideCursor();
        }
        //１秒に一回FPSを測定、UIに表示
        time += Time.deltaTime;
        if(time > 1)
        {
            FPS.text = 1/Time.deltaTime +  "fps";
            time = 0;
        }
    }

    // 各UI表示キーが押されたときに呼ばれる関数
    public void GetKeyEscape(int n)
    {
        Toggle1();//こいつを実行する

        foreach (var item in canvas)//一回、すべてのキャンバスを非表示にする
        {
            if(item.enabled == true)
            {
                item.enabled = false;
            }
        }
        
        if(n == 0)//nに応じたキャンバスを表示する
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

    public void ShowCursor()//カーソルを見せたいときに呼ぶ関数
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void HideCursor()//カーソルを隠したいときに呼ぶ関数
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ChangeCanvas(int n)//nに応じたキャンバスを表示させる関数
    {
        if(canvas[0].enabled == true)canvas[0].enabled = false;

        canvas[n].enabled = true;

        if(n == 1)itemsDialog.reflesh();

        if(n == 4)ChangeSetting(1);
    }

    public void BackPausePanel()//バックボタンが押されたときにポーズ画面に戻る関数　基本的にボタンから直接呼ばれる
    {
        foreach (var item in canvas)
        {
            item.enabled = false;
        }
        canvas[0].enabled = true;
    }
// おそらく、ここから下のいくつかのプログラムは使われていない
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

    public void Toggle1(){//ポーズ画面含む、ポーズ中に操作するＵＩのアクティブ状態を切り替える変数、同時に、タイムスケール、カーソル状態も変更

        PauseCanvas.SetActive(!PauseCanvas.activeSelf);//アクティブ状態を反転させる
        if(ChangeVolumeFlag == true)
        {
            ChangeVolumeFlag = false;
            admin_Date.SaveDateOther(9);//その時に、ボリューム設定を変更していたら、変更を保存する
        }

        if(ChangeSkillTree == true)
        {
            ChangeSkillTree = false;
            admin_Date.SaveDateOther(12);
            Debug.Log("スキルツリーを保存");
        }

        if (PauseCanvas.activeSelf)//ポーズ状態なら実行
        {
            Time.timeScale = 0;//タイムスケールを0に　⁼　時間停止
            virtualCamera.enabled = false;//カメラの回転を停止
            
            if(isWorkeingMobilePlatform == true)return;//パソコンの時のみ、カーソルを表示させる
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else//ポーズ状態を解除したとき
        {
            move.LastAttack();//アニメーションをリセット
            Time.timeScale = 1;//タイムスケールをもとに戻す　= 時は動き出す
            virtualCamera.enabled = true;//カメラの回転を復活させる

            if(isWorkeingMobilePlatform == true)return;//パソコンの時のみ、カーソルを隠す
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }        
    }

    //UIからの手動セーブの実行命令を受け取る関数
    public void Save()
    {
        admin_Date.SaveDate();
        admin_Date.SaveDateOther(0);
    }

    //UIからFPSの設定を変更されたとき呼ばれる関数
    public void ChangeFPS(int n)
    {
        NawFPS = n;
        Application.targetFrameRate = NawFPS;//ターゲットFPSを変更
        admin_Date.SaveDateOther(3);//個別データをセーブ
        //ボタンの色を変更
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
    
    //UIからBGMの設定を変更されたとき呼ばれる関数
    public void ChangeBGM(int n)
    {
        admin.ChangeBGM(n);//再生するBGMを変更
        BGMnumber = n;
        admin_Date.SaveDateOther(8);//個別データを保存
        //ボタンの色を変更
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

    // UIから感度の設定を変更されたとき呼ばれる関数
    public void ChangeAIMSpeed()
    {
        AIM_X_speed = float.Parse(aim_x_speed.text);//Ｘ感度を変更
        if(AIM_X_speed <= 0)//０以下の場合は強制的に1にする
        {
            AIM_X_speed = 1;
            aim_x_speed.text = "1";
        }
        AIM_Y_speed = float.Parse(aim_y_speed.text);//Ｙ軸感度を変更
        if(AIM_Y_speed <= 0)//０以下の場合は強制的に1にする
        {
            AIM_Y_speed = 1;
            aim_y_speed.text = "1";
        }

        if(isWorkeingMobilePlatform == true)//モバイルで動いているとき、バーチャルコントローラーのアイム感度を変更
        {
            roteCamera._angularPerPixel = new Vector2(AIM_X_speed , AIM_Y_speed)*CAMERA_ROTE_SPEED_MAGNIFICATION_TO_MOBILE;
        }
        else//パソコンで動いているとき、シネマシンのＰＯＶspeedを変更
        {
            cinemachinePOV.m_HorizontalAxis.m_MaxSpeed = AIM_X_speed;
            cinemachinePOV.m_VerticalAxis.m_MaxSpeed = AIM_Y_speed;
        }
        
        admin_Date.SaveDateOther(4);//個別データを保存
    }

    // それぞれのトグルが変更されたときに呼ばれる関数
    public void ChangeToggle(int n)
    {
        if(n == 0)
        {
            FullScreen = toggledates[0].Value;//フルスクリーン情報を変更
            admin_Date.SaveDateOther(5);//個別データを保存
            ChangeScreenResolution(ScreenResolution);//フルスクリーンにするかしないか
        }
        if(n == 1)
        {
            shadows = toggledates[1].Value;//影情報を変更
            admin_Date.SaveDateOther(7);//個別データを保存
            ChangeShadowBool();//影をつけるならつけろ
        }
        if(n == 2)
        {
            var s = toggledates[2].Value ? 0:1;//自動ダッシュ情報をintに変換する
            admin.ChangeAutoRun(s);//変換したint情報をadminに渡し、設定を変更
            admin_Date.SaveDateOther(10);//個別データを保存
        }
        if(n == 3)
        {
            admin_Date.AutoSave = toggledates[3].Value; //自動保存情報を変更
            admin_Date.SaveDateOther(11);//個別データを保存
        }
    }

    //フルスクリーン情報を変更するときに呼ばれる関数  多分きっと死んでいるプログラム　これの治し方わからん
    public void ChangeScreenResolution(int n)
    {
        Screen.SetResolution(Screen.currentResolution.width , Screen.currentResolution.height, FullScreen);//情報を変更
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

    //影情報を変更するときに呼ばれる関数
    public void ChangeShadowBool()
    {
        if(shadows == true)
        {
            Mainlight.shadows = LightShadows.Soft;//オンーーーーーーー
        }
        else
        {
            Mainlight.shadows = LightShadows.None;//オフ‐‐‐‐‐
        }
    }

    //設定画面内の個別設定を切り替える関数
    public void ChangeSetting(int n)
    {
        foreach (var item in SettingPanel)//一回、すべて消しましょう
        {
            item.SetActive(false);
        }
        //すべてのボタンを白色にしましょう
        image[6].color = Color.white;
        image[7].color = Color.white;
        image[8].color = Color.white;
        image[9].color = Color.white;

        SettingPanel[n-1].SetActive(true);//選択されているものだけ表示しましょう
        image[n + 5].color = new Color(0,0.3f,1);//選択されているものだけボタンの色を変えましょう
    }

    //モバイルの時武器を変更するときに呼ばれる関数　ＵＩからのみ実行される
    public void ChangeWeapon()
    {
        changeEquip.ChangeWeapon();//武器チェンジ
    }

    public void GetInput(int n)//moveにあるGetIntとの仲介役ですねmoveの説明見やがれください
    {
        move.GetInput(n);
    }
    
    //UIで音量情報が更新されたら呼ばれる関数
    public void ChangeVolume(int n)
    {
        if(n == 0)//マスターを変更
        {
            MasterVolume = slider[0].value;
            audioMixer.SetFloat("Master" , Mathf.Log10(MasterVolume) * 20);
        }
        else if(n == 1)//BGMを変更
        {
            BGMVolume = slider[1].value;
            audioMixer.SetFloat("BGM" , Mathf.Log10(BGMVolume) * 20);
        }
        else if(n == 2)//SEを変更
        {
            SEVolume = slider[2].value;
            audioMixer.SetFloat("SE" , Mathf.Log10(MasterVolume) * 20);
        }

        ChangeVolumeFlag = true;//これをオンにしないと一生保存されないぞ
    }

    //警告を出すときに呼ばれる関数
    public void Alarm(int n)
    {
        //わたされたintにおうじて、表示する警告を変えている
        Instantiate(alarmCanvas[n] , Vector3.zero , Quaternion.identity , AlarmParent.transform);
    }
}