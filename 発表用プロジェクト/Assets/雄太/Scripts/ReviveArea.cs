using UnityEngine;

public class ReviveArea : MonoBehaviour
{
    //このスクリプトは自動戦闘するキャラクターがダウン状態になったときに発生する蘇生エリアにアタッチする

    private Admin_Auto admin_Auto; //Admin_Autoを定義
    [SerializeField]
    private float revivePoint = 0;//復活までの値を定義　１になると復活できる

    //定数を定義
    const float REVIVE_LESS_POINT_PER_TIME = 0.07f;
    const float REVIVE_UP_POINT_PER_TIME = 0.02f;

    // Start is called before the first frame update
    void Start()
    {
        admin_Auto = GetComponentInParent<Admin_Auto>();//admin_Autoを格納
    }

    // Update is called once per frame
    void Update()
    {
        //revivePOintが0より大きいときは常に一定値が減っている
        if(revivePoint > 0)
        {
            revivePoint -=REVIVE_LESS_POINT_PER_TIME*Time.deltaTime;
            admin_Auto.hpSlider.value = revivePoint;
        }
        //１以上の時、キャラを復活させ、この蘇生エリアを削除する
        if(revivePoint >= 1)
        {
            admin_Auto.Revive();
            Destroy(this.gameObject);
            //復活エフェクトをつける
        }
    }

    void OnTriggerStay(Collider collider)//範囲内にplayerがいる場合、蘇生ポイントを上昇させる
    {
        if(!collider.CompareTag("Player"))return;
        revivePoint += REVIVE_UP_POINT_PER_TIME*Time.deltaTime;
    }

}
