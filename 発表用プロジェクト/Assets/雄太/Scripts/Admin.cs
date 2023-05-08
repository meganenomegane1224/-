using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;

public class Admin : MonoBehaviour
{
    [Header("ステータス") , Tooltip("主人公のステータス　　レベル以外は設定しても意味ないよ")]
        public static int CharaLevel;     //てきのレベルを定義     //０は武器なし
        public static float MainEXP;      //経験値を定義    //１は槍
        public static float AttackStatus;  //攻撃力を定義   //２は剣
        public static float HPStatus;     //HPを定義    //３は銃
        public static float DefenceStatus;//防御力を定義

    [Space(8)]

    [Header("設定オブジェクト") , Tooltip("このスクリプトで使うゲームオブジェクト、プレファブを格納する所")]
        [SerializeField]
        private Text[] Statust_UI;             //ステータスを表示するtextを配列化して定義
        // [SerializeField]
        // private Slider VolumeSlider;        
        [SerializeField]
        private Transform TalkButtonPanel;    //会話開始ボタンを表示するオブジェクトをアタッチ
        [SerializeField]
        ChangeEquip changeEquip;              //武器変更プログラムをアタッチ
        [SerializeField]
        private Animator animator;            //animatorをアタッチ
        [SerializeField]
        private RectTransform SelectEdge;     //会話の選択パネルをアタッチ
        [SerializeField]
        private Admin_Date admin_Date;        //admin_dateをアタッチ
    [Space(8)]


    [Header("設定変数") , Tooltip("変数を設定する所")]
        [SerializeField]
        private AudioClip[] BGM;            //BGMを配列化して定義     
        [SerializeField]
        public float LockDistance = 10;     //定数化
        public static int MainWeapon = 2;   //メイン武器ナンバーを定義
        public static int SubWeapon = 1;    //サブ武器ナンバーを定義
        public float LockOffDistance;       //定数化
 
    [Space(8)]

    [Header("状態表示") , Tooltip("状態が表示される　それ以上でもそれ以下でもない")]
        
        public bool LockOn;                 //敵をロックオンしているかしてないか
        public GameObject LockEnemy;        //ロックしている敵を定義　勝手に追加される
        public bool InTalkFlag;             //トークエリアにtalkEventが入っている
        public List<TalkEvent> talkObject;  //トークエリア内にいるtalkeventが配列化される
        public TalkEvent TalkTargetObj;     //現在話しているオブジェクトが追加される
        public static int WeaponNumber;     //現在装備している武器ナンバーを定義
        public  int skillPoints = 10;             //スキルを取得するのに必要なスキルポイントを格納
        


    private int LevelUPEXP;                  //次のレベルアップに必要な経験値総量を定義
    [SerializeField]
    private Move characterScript;            //アタッチだよ
    private AudioSource audioSource;         //アタッチだよぉ
    
    private SearchEnemy searchEnemy;         //アタッチだよぉぉ
    private Flowchart flowchart;             //以下略
    [HideInInspector]
    public Transform targetTransform;        //話をする相手を定義  
    // private Canvas EnemyHPCanvas;
    [SerializeField]
    private int SelectEdNumber;              //会話ボタン配列の何番目を選んでいるか定義

    // Start is called before the first frame update
    void Awake()
    {
        //保存されているパラメーター情報を読み込み、適用
        CharaLevel = PlayerPrefs.GetInt("LEVEL");
        MainEXP = PlayerPrefs.GetFloat("EXP");
        MainWeapon = PlayerPrefs.GetInt("MainW");
        SubWeapon = PlayerPrefs.GetInt("SubW");
        HPStatus = 80 + CharaLevel*20;
        AttackStatus = 8 + CharaLevel*2;
        LevelUPEXP = 10 * CharaLevel * CharaLevel + 10 * CharaLevel;
        DefenceStatus = CharaLevel * 0.005f;
        WeaponNumber = MainWeapon;
        characterScript = GetComponentInParent<Move>();
    }

    // Update is called once per frame
    void Start()
    {
        // それぞれのコンポーネントを適用
        flowchart = GetComponentInChildren<Flowchart>();
        audioSource = GetComponent<AudioSource>();
        searchEnemy = GetComponentInChildren<SearchEnemy>();
        
        //ステータスＵＩの内容を更新
        Statust_UI[0].text = string.Format("MaxHP " + HPStatus );
        Statust_UI[1].text = string.Format("Attack Power " + AttackStatus);
        Statust_UI[2].text = string.Format("Defence Power " + DefenceStatus);
        Statust_UI[3].text = string.Format("All EXP " + MainEXP);
        Statust_UI[4].text = string.Format("次レベル必要総経験値　"　+ LevelUPEXP);
        Statust_UI[5].text = string.Format("Level  " + CharaLevel);

        audioSource.clip = BGM[PlayerPrefs.GetInt("BGM")];//保存されていたＢＧＭをaudiosourceに設定
        audioSource.Play();//BGmを再生

        LockOn = false;
        ChangeAutoRun(PlayerPrefs.GetInt("AutoRun"));//自動ダッシュをするかしないか保存されたデータを読み込み
        skillPoints = PlayerPrefs.GetInt("skillPoint");
    }

    void Update()
    {

        if(Input.GetKeyDown("f"))//fキーを押すと選択する
        {
            Select();
        }
        
        //トークエリアにトークオブジェクトがいるとき、マウススクロールをすることで選択する会話ボタンを切り替え可能
        if(InTalkFlag == true && characterScript.state != Move.MyState.TalkEvent)
        {
            var n = -Input.GetAxis("Mouse ScrollWheel")*10;//スクロール量を取得し、10倍して整数化
            if(SelectEdNumber <= talkObject.Count - 1 && 0<=SelectEdNumber) 
            {
                SelectEdNumber += (int)n;
            }
            
            // 配列内のtalkeventを超えて移動した場合は範囲内に戻す
            if(SelectEdNumber < 0)
            {
                SelectEdNumber = 0;
            }           
            else if(SelectEdNumber > talkObject.Count-1)  // -250 -285 35間隔
            {
                SelectEdNumber = talkObject.Count - 1;
            }
            SelectEdge.anchoredPosition = new Vector3(160 , 15.5f - 35 *SelectEdNumber ,0);//セレクトされていることを示す画像を移動する
        }
    }

    // 武器変更等でanimatorをリセットしたいときに呼ばれる関数
    public void ReturnAccess()
    {    
        
        animator.SetFloat("0Speed" , 0);

        animator.ResetTrigger("1Attack");
        animator.ResetTrigger("2Attack");
        animator.SetFloat("1Speed", 0);
        animator.SetFloat("3Speed", 0);
        animator.SetFloat("2Speed", 0);
        animator.SetBool("1Idle", false);
        animator.SetBool("2Idle", false);
        characterScript.SetState(Move.MyState.Normal); //ノーマル状態にする
    }

    //敵を倒したときに経験値をもらう関数
    public void TakeEXP(float EXP)
    {
        MainEXP += EXP;//そう経験値を加算する

        // 総経験値がレベルアップ経験値より低くなるまでレベルアップ処理を行う
        while(MainEXP >= LevelUPEXP)
        {
            CharaLevel++;//キャラレベルを1上げる
            // 各ステータスを再計算
            HPStatus = 80 + CharaLevel*20;
            LevelUPEXP = 10 * CharaLevel * CharaLevel + 10 * CharaLevel;
            AttackStatus = 8 + CharaLevel*2;
            DefenceStatus = CharaLevel * 0.005f;
            characterScript.LevelUP();//moveを通じてHPを回復させる
        }


        admin_Date.SaveDateOther(1);//データをセーブする
        //UIにステータスを表示する
        Statust_UI[0].text = string.Format("MaxHP " + HPStatus );
        Statust_UI[1].text = string.Format("Attack Power " + AttackStatus);
        Statust_UI[2].text = string.Format("Defence Power " + DefenceStatus);
        Statust_UI[3].text = string.Format("All EXP " + MainEXP);
        Statust_UI[4].text = string.Format("次レベル必要総経験値　"　+ LevelUPEXP);
        Statust_UI[5].text = string.Format("Level  " + CharaLevel);
    }

    // BGMを変更するプログラム　基本的にUIから直接呼ばれる
    public void ChangeBGM(int BGMnumber)
    {
        if(audioSource.clip != BGM[BGMnumber])
        {
            audioSource.clip = BGM[BGMnumber];//BGMを設定
            audioSource.Play();            //BGMを再生
        }
    }

    // public void Volum()
    // {
    //     audioSource.volume = VolumeSlider.value;
    // }

    // 敵をロックするプログラム　未ロックで攻撃時に呼ばれる
    public void LockOnEnemy()
    {
        LockEnemy = searchEnemy.Search(); //一番近くにいる
        if(LockEnemy == null) return;     //からの場合は実行しない
        

        float dis = Vector3.SqrMagnitude(LockEnemy.transform.position-this.transform.position);//近いオブジェクトとの距離を計算
        var adminEnemy = LockEnemy.GetComponent<Admin_EnemyStatus>();//敵のadmin_enemystatusコンポーネントを取得
        // ロック可能距離内でかつ、トークフラッグがfalseなら
        if(dis <= LockDistance*LockDistance && adminEnemy.TalkFalg() == false && adminEnemy.HP > 0)
        {
            adminEnemy.LockedOn();//ロックを実行
            LockOn = true;
        }
        else
        {
            LockEnemy = null;
        }
    }

    // すでにロック状態下で再度ロック判定を行う関数　　動作不安定
    public void SecondLockOn()
    {
        
        // Ray ray = new Ray(transform.position , characterScript.velocity);
        RaycastHit hit;//レイを飛ばす
        //レイに当たったオブジェクトが敵なら再度ロックオンを行う
        if(Physics.BoxCast(transform.position , new Vector3(2,0.5f,0) , transform.forward , out hit , Quaternion.identity , 4)&& hit.collider.tag == "Enemy")
        {
            if(hit.collider.gameObject == LockEnemy) return;//すでにロックしている敵なら実行しない
            var s =  hit.transform.GetComponent<Admin_EnemyStatus>();
            s.LockedOff();
            if(s.TalkFalg() == true) return;
            LockEnemy = s.gameObject;
            s.LockedOn();
        }
    }

    //ロック状態を解除する
    public void LockOff()
    {
        if(LockEnemy != null)
        {
            LockEnemy.GetComponent<Admin_EnemyStatus>().LockedOff();
        }
       
        LockOn = false;
        LockEnemy = null;
    }

    // メイン武器を切り替えるときに呼ばれる関数
    public void ChangeMainWeapon(int number)
    {
        MainWeapon = number;
        ReturnAccess();
        changeEquip.ChangeWeapon();
    }

    // サブ武器を切り替えるときに呼ばれる関数
    public void ChangeSubWeapon(int number)
    {
        SubWeapon = number;
        ReturnAccess();
        changeEquip.ChangeWeapon();
        admin_Date.SaveDateOther(2);
    }


    // 会話をするときに呼ばれる関数
    public void Talk(int n , TalkEvent talkEvent)
    {
        flowchart.SendFungusMessage("Talk" + n);//fungusのフローチャートの渡されたint番号の会話を実行する
        targetTransform = talkEvent.transform;//相手の位置を更新する
        TalkTargetObj = talkEvent;//会話相手を更新する
        characterScript.SetState(Move.MyState.TalkEvent);//会話状態にする
        TalkButtonPanel.gameObject.SetActive(false);//talkButtonを消す
        SelectEdge.gameObject.SetActive(false);//選択画像を消す
    }


    // 会話が終わったときに呼ばれる関数
    public  void TalkEnd()
    {
        characterScript.SetState(Move.MyState.Normal);//状態をノーマルにする
        TalkTargetObj.TalkOff();//会話相手に会話終了を伝える
        TalkTargetObj = null;
        targetTransform = null;
        TalkButtonPanel.gameObject.SetActive(true);//talkButtonを再表示する
        SelectEdge.gameObject.SetActive(true);//選択画像を再表示する
    }

    //会話オブジェクトが会話可能範囲に入ったときに呼ぶ関数
    public void InTalkRange(TalkEvent obj)
    {
        talkObject.Add(obj);//会話オブジェクトを配列に加える
        RefreshTalkButton();//関数を呼ぶ
        if(InTalkFlag == false)//もしfalseならボタンを表示して、セレクトナンバーを0にする
        {
            InTalkFlag =true;
            SelectEdge.gameObject.SetActive(true);
            SelectEdNumber = 0;
        }
    }

    // 会話オブジェクトが会話可能範囲から出たときに呼ぶスクリプト
    public void OutTalkRange(TalkEvent obj)
    {
        talkObject.Remove(obj);//会話オブジェクトを配列から除外する
        RefreshTalkButton();//リフレッシュぅぅ
        if(talkObject.Count == 0)//会話オブジェクト数が0になったら消してけす
        {
            InTalkFlag =false;
            SelectEdge.gameObject.SetActive(false);
        }
    }

    // 会話オブジェクトのボタンを背列する関数
    public void RefreshTalkButton()
    {
        foreach(Transform child in TalkButtonPanel.transform)//すべてのボタンを削除する
        {
            Destroy(child.gameObject);
        }
        var Count = talkObject.Count;
        for (int i = 0; i < Count; i++)//再度、更新されたボタンを再表示する
        {
            Instantiate(talkObject[i].talkStartButton.gameObject, new Vector3(0,0,0), Quaternion.Euler(0,0,0), TalkButtonPanel);
        }
    }

    // トークオブジェクトがあるときに呼ばれると会話を開始する関数
    private void Select()
    {
        if(InTalkFlag == false)return;
        talkObject[SelectEdNumber].TalkStart();
    }

    // 範囲内の当たり判定に敵発生オブジェクトが入った場合に呼ばれる関数
    void OnTriggerStay(Collider col) 
    {
        if(col.tag != "Appear")return;

        var appearScript = col.GetComponent<AppearScript>();//スクリプトを取得
        if(appearScript.encountCharaFlag == false)//falseの場合、trueにする関数を呼ぶ
        {
            appearScript.CharacterConnect();
        }
        else         //trueの場合、そいつに自分の座標をリアルタイムで送信する
        {
            appearScript.CharacterPosition = transform.parent.transform.position;
        }
    }

    // 範囲内の当たり判定に敵発生オブジェクトから出た場合に呼ばれる関数
    void OnTriggerExit(Collider col) 
    {
        if(col.tag != "Appear")return;
            col.GetComponent<AppearScript>().CharacterDisconnect();//さようならだ
    }

    // 自動ダッシュをするかしないか切り替える関数
    public void ChangeAutoRun(int n)
    {
        if(n == 0)
        {
            characterScript.AutoRunFlag = true;//します！
        }
        else if(n == 1)
        {
            characterScript.AutoRunFlag = false;//するわけねーだろばーーか
        }
        
    }

    public bool isWorkeingMobileImage()
    {
        return characterScript.isWorkeingMobilePlatform;
    }

    public void RaiseHPBySkillTree()
    {
        var n = Admin_SkillTree.AllRaiseHP;
        HPStatus += n;
        characterScript.charahp += n;
        characterScript.slider.value = characterScript.charahp/HPStatus;
    }

    public static float LastAttackStatus()
    {
        return AttackStatus+Admin_SkillTree.AllRaiseAttack;
    }
    public static float LastDefenceStatus()
    {
        return DefenceStatus + Admin_SkillTree.AllRaiseDefence;
    }
}