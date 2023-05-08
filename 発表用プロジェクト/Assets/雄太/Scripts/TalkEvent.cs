using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//このスクリプトは会話等の選択画面をだす場合に使うもの。item,Admin_Chestをアタッチするかしないかで実行するプログラムが変化する
public class TalkEvent : MonoBehaviour
{
    [SerializeField]
    public TalkStartButton talkStartButton;         // 会話の開始を感知するスクリプトがついたボタンをアタッチ
    [SerializeField]
    private Admin admin;                            //adminをアタッチ
    [SerializeField]
    private int talkNumber;                         //このスクリプトが、会話をする場合、その会話の番号を設定する場所。この番号はfungsの番号と連動している
    private GameObject EnemyObj;                    //このスクリプトの親オブジェクトを定義
    private Vector3 velocity;
    private Quaternion targetRotation;              //会話の時に向く方向を格納するQuaternion変数
    private GameObject TargetObj;                   //話をするときの相手のgameObjectを格納
    [SerializeField]
    private Vector3 FirstVelocity;                  //話す前に敵の方向を格納
    [SerializeField]
    private Item item;                              //このtalkEventがアイテムを拾うためのものの場合、itemスクリプトをアタッチする


    [SerializeField]
    private Admin_Chest admin_Chest;                //このtalkEventがチェストを開くためのものなら、Chest管理スクリプトをアタッチする

    //定数を宣言
    const float ROTATION_SPEED = 300;


    // Start is called before the first frame update
    void Start()
    {
        if(item == null)                 // これが、話をするための場合、親オブジェクトを取得する
        {
            EnemyObj = transform.parent.gameObject;
        }
        if(admin == null)                //adminコンポーネントがアタッチされていない場合は、シーン内からadminを探してアタッチする
        {
            admin = GameObject.Find("管理").GetComponent<Admin>(); 
        }
        talkStartButton.parentObj = this;
    }
    
    void Update()
    {
        if(item != null || admin_Chest != null)return;      //これが話をするための場合、親オブジェクトを常にtargetRotaionの方向に向ける
        EnemyObj.transform.rotation = Quaternion.RotateTowards(EnemyObj.transform.rotation , targetRotation , ROTATION_SPEED*Time.deltaTime);
    }
    void OnTriggerStay(Collider col) //このオブジェクトの当たり判定内にadminコンポーネントを含むオブジェクトがあるとき、会話ボタンを表示させるプログラムを起動する
    {
        if(col.tag == "Player" && admin.talkObject.Contains(this) == false)//あたり判定内のオブジェクトが"Player"かつすでにadmin.talkObject内に定義されてるか判定
        {
            admin.InTalkRange(this);
            
            TargetObj = col.gameObject;
        }
    }
    void OnTriggerExit(Collider col)
    {
        if(col.tag == "Player" && admin.talkObject.Contains(this) == true)//当たり判定内からadminが外れたら、talkObjectから自信を除外する
        {
            admin.OutTalkRange(this);
            TargetObj = null;
        }
    }

    public void TalkStart()//会話を開始させるプログラム。ボタンからの作動と、Fキーからによる作動がある
    {
        if(item == null && admin_Chest == null)//会話の場合、adminの会話開始プログラムを作動させる
        {
            admin.Talk(talkNumber , this);
            velocity = -EnemyObj.transform.position + TargetObj.transform.position;//会話中に向かせる方向を設定
            targetRotation = Quaternion.LookRotation(new Vector3(velocity.x , 0 , velocity.z));//向いている方向を変更
        }
        else if(item != null && admin_Chest == null)//itemを拾うプログラムの場合、アイテムを拾うプログラムを実行する
        {
            item.GetItem();
            admin.OutTalkRange(this);
            //削除判定はitemのほうで実行
        }
        else if(admin_Chest != null)//chetを開く場合、開くプログラムを実行する
        {
            admin_Chest.Open();
            admin.OutTalkRange(this);
            Destroy(this.gameObject);
        }
        
    }

    public void TalkOff()    //会話終了後、むきを戻す
    {
        velocity = Vector3.zero;
        targetRotation = Quaternion.LookRotation(FirstVelocity);
    }
}