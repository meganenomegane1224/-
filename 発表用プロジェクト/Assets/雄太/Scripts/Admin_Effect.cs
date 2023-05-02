using UnityEngine;
using Effekseer;

public class Admin_Effect : MonoBehaviour
{
    [SerializeField]
    private bool isDamageEffect = true;

    [SerializeField , Header("ダメージ倍率")]
    private float DamageMagnification = 1;

    [Space(5)]

    [SerializeField , Tooltip("連続ダメージを発生させるかどうか")]
    private bool ContinueDamage = false;
    
    [SerializeField , Tooltip("連続ダメージの発生間隔")]
    private float DamageInterval = 0.2f;
    public FirstTransForm[] firstTransForm;
    [SerializeField]
    private float time;
    [HideInInspector]
    public int EffectType = 0;
    // [SerializeField]
    private EffekseerEmitter effekseerEmitter;
    



    [System.Serializable]
    public struct FirstTransForm
    {
        public Vector3 position;
        public Vector3 Addrotation;
        public Vector3 scale;
    }

    void Awake()
    {
        effekseerEmitter = GetComponent<EffekseerEmitter>();
        if(isDamageEffect == false)
        {
            Destroy(GetComponent<Collider>());
        }
    }
    public void EffectStart(int n)
    {
        var parent = GetComponentInParent<Transform>();
        transform.rotation = transform.parent.rotation;
        transform.localPosition = firstTransForm[n].position;
        transform.localScale = firstTransForm[n].scale;
        transform.Rotate(firstTransForm[n].Addrotation);
        time = DamageInterval;
        effekseerEmitter.Play();
    }

    public void EffectEnd()
    {
        effekseerEmitter.Stop();
    }

    void OnTriggerEnter(Collider collider)
    {
        if(ContinueDamage == false && collider.CompareTag("Enemy"))
        {
            var damage = Admin.AttackStatus * DamageMagnification;
            Debug.Log(collider + "に" + damage + "だめーじ");
            collider.GetComponent<Admin_EnemyStatus>().TakeDamage(damage);
            
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if(ContinueDamage == true && collider.CompareTag("Enemy"))
        {
            time += Time.deltaTime;
            Debug.Log("連続攻撃");
            if(time > DamageInterval)
            {
                var damage = Admin.AttackStatus*DamageMagnification;
                collider.GetComponent<Admin_EnemyStatus>().TakeDamage(damage);
                time = 0;
                
            }
        }
    }
}
