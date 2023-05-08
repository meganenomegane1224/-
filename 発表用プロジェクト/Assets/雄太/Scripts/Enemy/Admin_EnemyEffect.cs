using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Effekseer;

public class Admin_EnemyEffect : MonoBehaviour
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
    float damage;
    [SerializeField]
    private bool emitterFlag = true;
    private Collider coll;
    [SerializeField]
    private float ColliderONTime = 0;
    private float ColliderOFFTime;
    [SerializeField]
    private int repetitionCount = 1;

    [System.Serializable]
    public struct FirstTransForm
    {
        public Vector3 position;
        public Vector3 Addrotation;
        public Vector3 scale;
    }

    void Awake()
    {
        if(emitterFlag == true)
        {
            effekseerEmitter = GetComponent<EffekseerEmitter>();
        }
        if(isDamageEffect == false)
        {
            Destroy(GetComponent<Collider>());
        }
    }

    void Start()
    {
        damage = GetComponentInParent<Admin_EnemyStatus>().AttackStatus * DamageMagnification;
        coll = GetComponent<Collider>();
        if(ColliderONTime > 0)
        {
            coll.enabled = false;
            StartCoroutine(ColliderOn());
        }
    }

    IEnumerator ColliderOn()
    {
        var x = new WaitForSeconds(ColliderONTime);
        yield return x;
        coll.enabled = true;
    }
    
    public void EffectStart(int n)
    {
        var parent = GetComponentInParent<Transform>();
        transform.rotation = transform.parent.rotation;
        transform.localPosition = firstTransForm[n].position;
        transform.localScale = firstTransForm[n].scale;
        transform.Rotate(firstTransForm[n].Addrotation);
        time = DamageInterval;
        if(emitterFlag == true)
        {
            for (int i = 0; i < repetitionCount + 1; i++)
            {
                effekseerEmitter.Play();
            }
        }
    }

    public void EffectEnd()
    {
        if(emitterFlag == true)
        {
            effekseerEmitter.Stop();
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if(ContinueDamage == true)return;

        if(collider.CompareTag("Player"))
        {
            collider.GetComponent<Move>().TakeDamage(damage);
            Debug.Log(collider + "に" + damage + "だめーじ");
        }
        if(collider.CompareTag("AutoChara"))
        {
            collider.GetComponent<Admin_Auto>().TakeDamage(damage);
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if(ContinueDamage == false)return;

        if(collider.CompareTag("Player"))
        {
            time += Time.deltaTime;
            
            if(time > DamageInterval)
            {
                collider.GetComponent<Move>().TakeDamage(damage);
                time = 0;
                Debug.Log("連続攻撃");
            }
        }
        if(collider.CompareTag("AutoChara"))
        {
            if(time > DamageInterval)
            {
                collider.GetComponent<Admin_Auto>().TakeDamage(damage);
            }
        }
    }
}
