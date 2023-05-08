using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Admin_Date : MonoBehaviour
{

    [Header("アタッチスクリプト")]
        [SerializeField]
        private Move move;           //アタッチ
        [SerializeField]
        private Admin admin;         //アタッチ
        [SerializeField]
        private RotatoSun rotatoSun; //アタッチ
        [SerializeField]
        private ChangeEquip changeEquip;//アタッチ
        [SerializeField]      
        private Admin_UI admin_UI;    //アタッチ
        [SerializeField]
        private Admin_SkillTree admin_SkillTree;//アタッチ

    [Space(6)]

    [Header("その他")]
        [SerializeField]
        private float SaveTime = 30;  //位置情報などの時間軸的に変化するデータの保存を行う秒数
        [SerializeField]
        private Vector3 FirstCharaPOsition;//ゲーム初回起動時の位置を設定
        [SerializeField]
        public bool AutoSave = true;   //自動セーブをするかしないか

    private float time;               //セーブカウントを定義

    private void Awake() 
    {
        if(PlayerPrefs.GetFloat("Sun_x") == 0)//初期位置から判断して、ゲームが初回起動と判断されたなら各保存情報を設定する
        {
            move.transform.position = FirstCharaPOsition;//初期位置を設定
            Admin.CharaLevel = 1;//レベルを設定
            Admin.MainEXP = 0;//そうHPを設定

            rotatoSun.FirstRotate();//太陽の初期回転を設定
            admin.ChangeMainWeapon(2);//メイン武器を設定
            admin.ChangeSubWeapon(3);//サブ武器を設定
            
            admin_UI.NawFPS = 60;//ターゲットFPSを設定
            admin_UI.FullScreen = true;//フルスクリーン設定を設定（？）
            admin_UI.shadows = false;//影のオンオフを設定
            admin_UI.ScreenResolution = 1;//フルスクリーンのオンオフを設定
            admin_UI.MasterVolume = 1;//マスターボリュームを設定
            admin_UI.BGMVolume = 1;//BGMボリュームを設定
            admin_UI.SEVolume = 1;//SEボリュームを設定
            admin.skillPoints = 6;

            if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                //スマホなら以下のように感度を設定
                admin_UI.AIM_X_speed = 4;
                admin_UI.AIM_Y_speed = 16;
            }
            else
            {
                //パソコンなら以下のように感度を設定
                admin_UI.AIM_X_speed = 6;
                admin_UI.AIM_Y_speed = 6;
                Debug.Log("パソコンで新規設定");
            }

            SaveDate();
            SaveDateOther(0);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(AutoSave == false)return;
        
        time += Time.deltaTime;
        if(time > SaveTime)
        {
            SaveDate();
        }
    }

    public void SaveDate()
    {
        //キャラクターポジション情報を保存
        var posi =  move.transform.position;
        PlayerPrefs.SetFloat("posi_X" , posi.x);
        PlayerPrefs.SetFloat("posi_y" , posi.y);
        PlayerPrefs.SetFloat("posi_z" , posi.z);
        //太陽位置、時間情報を保存
        var rote = rotatoSun.transform.rotation.eulerAngles;
        PlayerPrefs.SetFloat("Sun_x" , rote.x);
        PlayerPrefs.SetFloat("Sun_y" , rote.y);
        PlayerPrefs.SetFloat("Sun_z" , rote.z);
        PlayerPrefs.SetFloat("TimeRote" , rotatoSun.rote);

        PlayerPrefs.Save();//セーブを実行
        time = 0;
        Debug.Log("データをセーブしました");
    }

    // 個別データを保存する関数
    public void SaveDateOther(int n)
    {
        if(n == 0 || n == 1)//経験値、レベルを保存
        {
            //主人公parameter
            PlayerPrefs.SetFloat("EXP", Admin. MainEXP);
            PlayerPrefs.SetInt("LEVEL", Admin.CharaLevel);
        }

        if(n == 0 || n == 2)//装備済み武器情報を保存
        {
            PlayerPrefs.SetInt("MainW" , Admin.MainWeapon);
            PlayerPrefs.SetInt("SubW" , Admin.SubWeapon);
        }

         if(n == 0 || n == 3)
        {
            PlayerPrefs.SetInt("FPS" , admin_UI.NawFPS);
        }

        if(n == 0 || n == 4)
        {
            PlayerPrefs.SetFloat("Aim_X" , admin_UI.AIM_X_speed);
            PlayerPrefs.SetFloat("Aim_Y" , admin_UI.AIM_Y_speed);
        }
        if(n == 0 || n == 5)
        {
            PlayerPrefs.SetInt("FullScreen" ,admin_UI.FullScreen? 0:1);
        }
        if(n == 0 || n == 6)//フルスクリーン譲歩を保存
        {
            PlayerPrefs.SetInt("ScreenResolution" , admin_UI.ScreenResolution);
        }
        if(n == 0 || n == 7)//影情報を保存
        {
            PlayerPrefs.SetInt("Shadows" , admin_UI.shadows?0:1);
        }

        if(n == 0 || n == 8)//ﾅﾆｺﾚ
        {
            PlayerPrefs.SetInt("BGM" , admin_UI.BGMnumber);
        }
        if(n == 0 || n == 9)//音量情報を保存
        {
            PlayerPrefs.SetFloat("MVolume" , admin_UI.MasterVolume);
            PlayerPrefs.SetFloat("BGMVolume" , admin_UI.BGMVolume);
            PlayerPrefs.SetFloat("SEVolume" , admin_UI.SEVolume);
        }
        if(n == 0 || n == 10)//自動ダッシュを保存
        {
            PlayerPrefs.SetInt("AutoRun" , move.AutoRunFlag? 0:1);
        }
        if(n == 0 || n == 11)//自動保存設定を保存
        {
            PlayerPrefs.SetInt("AutoSave" , AutoSave? 0:1);
        }
        if(n == 0 || n == 12)
        {
            for (int i = 0; i < admin_SkillTree.skillsTreeUnites.Length; i++)
            {
                for (int h = 0; h < admin_SkillTree.skillsTreeUnites[i].skills.Length; h++)
                {
                    if(admin_SkillTree.GetSkillAvtiveSelf(i,h) == false)
                    {
                        Debug.Log(admin_SkillTree.GetSkillAvtiveSelf(i,h));
                    PlayerPrefs.SetInt(i.ToString() , h -1);
                    Debug.Log("スキルの幹は" + i + "で保存されたスキル番号は" + (h-1));
                    break;
                    }
                    if(h == admin_SkillTree.skillsTreeUnites[i].skills.Length-1 && admin_SkillTree.GetSkillAvtiveSelf(i,h) == true)
                    {
                        PlayerPrefs.SetInt(i.ToString() , h);
                        Debug.Log("スキルツリーの枝の最大値まで行きました。");
                        break;
                    }
                    
                }
            }

            PlayerPrefs.SetInt("skillPoint" , admin.skillPoints);
        }

        PlayerPrefs.Save();//セーブを実行
        Debug.Log("個別情報を保存しました");
    }
}
