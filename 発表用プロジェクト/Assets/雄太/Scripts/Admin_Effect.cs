using UnityEngine;
using Effekseer;
using DG.Tweening;

public class Admin_Effect : MonoBehaviour
{
    [SerializeField]
    private bool isDamageEffect = true;          //敵にダメージを与えることができるエフェクトなのか定義
    //コライダーを動かす場合は、コライダーをprefab化して別に用意し、それにEffectColliderSenderをアタッチし、特定の変数をインスペクターから設定すること
    [SerializeField]
    private bool canMoveCollider = false;        //コライダーが移動する処理をする場合はここにチェック
    [SerializeField]
    private ParticleSystem effect;               //ParticleSystemを使ってエフェクトを再生するときはアタッチする

    [SerializeField , Header("ダメージ倍率")]
    private float DamageMagnification = 1;        //攻撃力に対してどれくらいの倍率をかけるか定義

    [Space(5)]

    [SerializeField , Tooltip("連続ダメージを発生させるかどうか")]
    private bool ContinueDamage = false;          //連続ダメージを発生させるか定義
    
    [SerializeField , Tooltip("連続ダメージの発生間隔")]
    private float DamageInterval = 0.2f;          //連続ダメージの間隔を定義
    public FirstTransForm[] firstTransForm;       //エフェクトが発生する位置情報を配列化　複数のパターンを保存可能
    [SerializeField]
    private float time;                           //連続攻撃時間を定義
    // [SerializeField]
    private EffekseerEmitter effekseerEmitter;    //アタッチ
    [SerializeField]
    private GameObject colliderObj;               //コライダーを動かく場合、動かすコライダーがついたprefabがアタッチされる
    [SerializeField]
    private Vector3 colliderMoveTransform = Vector3.zero;//　コライダーが移動する移動先のlocalPosition
    [SerializeField]
    private float colliderMoveTime = 0f;//コライダーが動く場合、移動する時間
    
    


    // 位置情報にposition,rotation,scaleを設定する
    [System.Serializable]
    public struct FirstTransForm
    {
        public Vector3 position;
        public Vector3 Addrotation;
        public Vector3 scale;
    }

    void Awake()
    {
        if(GetComponent<EffekseerEmitter>())
        {
            effekseerEmitter = GetComponent<EffekseerEmitter>();//取得
        }
        if(isDamageEffect == false)
        {
            Destroy(GetComponent<Collider>());//ダメージエフェクト出ないなら当たり判定消す
        }
    }

    //エフェクトを再生するときに呼ばれる関数
    public void EffectStart(int n)
    {
        var parent = GetComponentInParent<Transform>();//親の位置を取得
        transform.rotation = transform.parent.rotation;//回転を親に合わせる
        transform.localPosition = firstTransForm[n].position;//ローカルポジションを変更
        transform.localScale = firstTransForm[n].scale;//スケール変更
        transform.Rotate(firstTransForm[n].Addrotation);//回転
        if(canMoveCollider == true)
        {
            colliderObj.transform.localPosition = Vector3.zero;//コライダーの位置を初期化
        }
        
        time = DamageInterval;

        if(effekseerEmitter != null)
        {
            effekseerEmitter.Play();//エフェクトを再生
        }
        else
        {
            effect.Play();//エフェクトを再生
        }

        if(canMoveCollider == true)//コライダーが移動可能なとき、コライダーを設定した位置に設定した時間で移動する
        {
            var g = Instantiate(colliderObj,transform.position,transform.rotation,this.transform);
            g.transform.DOLocalMove(colliderMoveTransform , colliderMoveTime);
            Destroy(g,colliderMoveTime);
        }        
    }

    public void EffectEnd()
    {
        effekseerEmitter.Stop();//エフェクトを停止
    }

    // ダメージエフェクトで範囲内に敵がいる場合に実行される関数
    public void OnTriggerEnter(Collider collider)
    {        
        if(collider.tag == "Enemy" &&ContinueDamage == false)
        {
            var damage = Admin.LastAttackStatus() * DamageMagnification + Admin_SkillTree.AllRaiseDamage;//ダメージ量を計算
            Debug.Log(collider + "に" + damage + "だめーじ");
            Debug.Log(Admin.LastAttackStatus());
            Debug.Log(Admin_SkillTree.AllRaiseDamage);
            collider.GetComponent<Admin_EnemyStatus>().TakeDamage(damage);//相手にダメージを与えるよ
        }
    }

    //ダメージエフェクトで連続ダメージがオンで範囲内に敵がいるときに実行される関数
    public void OnTriggerStay(Collider collider)
    {        

        if(ContinueDamage == true && collider.CompareTag("Enemy"))
        {
            time += Time.deltaTime;//時間を増やすよ
            if(time > DamageInterval)//時間になったら攻撃をするよ
            {
                var damage = Admin.LastAttackStatus()*DamageMagnification;//ダメージ量を計算
                collider.GetComponent<Admin_EnemyStatus>().TakeDamage(damage);//相手にダメージを与えるよ
                time = 0;//時間をリセットするよ
            }
        }

    }
}
