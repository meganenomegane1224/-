using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Admin_EnemyStatus : MonoBehaviour
{
    [Header("キャラタイプ")]
        [SerializeField]
        private EnemyType type;
    [Header("キャラレベル") , Tooltip("最も基本的な変数　レベルを変更するとすべてのステータス（Speed,Defence以外）の数値が上昇する")]
    [SerializeField]
    public int EnemyLevel = 1;

    [Space(8)]
    [Header("キャラステータス") , Tooltip("ここの変数は変更しても意味がないよ　ステータスを上昇させたいなら下の上昇数値でレベルを上げるか、ステータスごとに上昇値を設定してね")]
        public float HPStatus;
        public float AttackStatus;
        public float DefenceStatus;

    [Space(8)]

    [Header("ステータス上昇値") , Tooltip("ステータスの上昇値を設定する場所　Speedを０にすると全く動かなくなるよ")]
        [SerializeField]
        private float RiseHPStatus;
        [SerializeField]
        private float RiseAttackStatus;
        [SerializeField]
        private float RiseDefenceStatus;
        [SerializeField]
        public float MoveSpeed = 1;

    [Space(14)]
           
    [SerializeField , Tooltip("こいつが死んだときに主人公に与える経験値")]
    private float exp = 10;  
    [SerializeField , Tooltip("効果音を設定する所")]
    private AudioClip[] audioClip;


    [HideInInspector]
    public float HP;
    private AppearScript appearScript;

    private Admin_Enemy adminEnemy;
    private Admin_Enemy_G adminEnemyG;
    private Admin_Enemy_D adminEnemyD;
    private AudioSource audioSource;
    private Admin admin;
    [HideInInspector]
    public int appearNumber;
    [SerializeField]
    public Slider HPSlider;
    [HideInInspector]
    public float NowHPRatio;


    private enum EnemyType
    {
        Ghost,
        Golem,
        IronDog
    }

    void Awake()
    {
        HPStatus = EnemyLevel*20 + RiseHPStatus;
        AttackStatus = 10 + EnemyLevel*4 + RiseAttackStatus;   
        DefenceStatus = EnemyLevel*0.5f + RiseDefenceStatus;
    }

    // Start is called before the first frame update
    void Start()
    {
        admin = GameObject.Find("管理").GetComponent<Admin>();
        audioSource = GetComponent<AudioSource>();
        if(type == EnemyType.Ghost)
        {
            adminEnemy = GetComponent<Admin_Enemy>();
            adminEnemyG = null;
            adminEnemyD = null;
        }
        else if(type == EnemyType.Golem)
        {
            adminEnemyG = GetComponent<Admin_Enemy_G>();
            adminEnemy = null;
            adminEnemyD = null;
        }
        else if (type == EnemyType.IronDog)
        {
            adminEnemyD = GetComponent<Admin_Enemy_D>();
            adminEnemy = null;
            adminEnemyG = null;
        }
        
        appearScript = GetComponentInParent<AppearScript>();
        HP = HPStatus;
        // NowHPRatio = HP/HPStatus;
        HPSlider.value = 1;
    }


    public void TakeDamage(float damage)
    {

        if(type == EnemyType.Ghost)
        {
            if(adminEnemy.TalkEventFlag == true || HP <= 0) return;
            HP -= damage;
            // NowHPRatio = HP/HPStatus;
            audioSource.PlayOneShot(audioClip[0]);

            HPSlider.value = HP/HPStatus;
            
            // if(adminEnemy.LockedFlag == true)
            // {
            //     admin.ChangeEnemyHP(NowHPRatio);
            // }
            if(adminEnemy.invincibleFlag == false && HP > 0)
            {
                adminEnemy.SetState(Admin_Enemy.EnemyState.Damage);
            }
            if(HP <= 0)// && adminEnemy.state != Admin_Enemy.EnemyState.Die
            {
                adminEnemy.SetState(Admin_Enemy.EnemyState.Die);
                if(adminEnemy.LockedFlag == true)
                {
                    admin.LockOff();
                }
            }
        }
        else if(type == EnemyType.Golem)
        {
            if(adminEnemyG.TalkEventFlag == true || HP <= 0) return;
            HP -= damage;
            // NowHPRatio = HP/HPStatus;
            HPSlider.value = HP/HPStatus;

            // if(adminEnemyG.LockedFlag == true)
            // {
            //     admin.ChangeEnemyHP(NowHPRatio);
            // }
            if(adminEnemyG.invincibleFlag == false && HP > 0)
            {
                adminEnemyG.SetState(Admin_Enemy_G.EnemyState.Damage);
            }
            else if(HP<=0)
            {
                adminEnemyG.SetState(Admin_Enemy_G.EnemyState.Die);
                if(adminEnemyG.LockedFlag == true)
                {
                    admin.LockOff();
                }
            }
            audioSource.PlayOneShot(audioClip[0]);
        }
        else if(type == EnemyType.IronDog)
        {
            if(adminEnemyD.TalkEventFlag == true || HP <= 0) return;
            HP -= damage;
            // NowHPRatio = HP/HPStatus;
            HPSlider.value = HP/HPStatus;

            // if(adminEnemyG.LockedFlag == true)
            // {
            //     admin.ChangeEnemyHP(NowHPRatio);
            // }
            if(adminEnemyD.invincibleFlag == false && HP > 0)
            {
                adminEnemyD.SetState(Admin_Enemy_D.EnemyState.Damage);
            }
            else if(HP<=0)
            {
                adminEnemyD.SetState(Admin_Enemy_D.EnemyState.Die);
                if(adminEnemyD.LockedFlag == true)
                {
                    admin.LockOff();
                }
            }
            audioSource.PlayOneShot(audioClip[0]);
        }


    }

    private void DestroyEvent()
    {
        var drop = GetComponent<MobItemDropper>();
        drop.DropIfNeeded();
        admin.TakeEXP(exp);
        Debug.Log("死亡");

        appearScript.EnemyDie(appearNumber);
        Reset();
    }

    public void LockedOn()
    {
        if(type == EnemyType.Ghost)
        {
            adminEnemy.LockedFlagImage.enabled = true;
            adminEnemy.LockedFlag = true;
        }
        else if (type ==EnemyType.Golem)
        {
            adminEnemyG.LockedFlagImage.enabled = true;
            adminEnemyG.LockedFlag = true;
        }
        else if(type == EnemyType.IronDog)
        {
            adminEnemyD.LockedFlagImage.enabled = true;
            adminEnemyD.LockedFlag = true;
        }
    }
    public void LockedOff()
    {
        if(type == EnemyType.Ghost)
        {
            adminEnemy.LockedFlagImage.enabled = false;
            adminEnemy.LockedFlag = false;
        }
        else if(type == EnemyType.Golem)
        {
            adminEnemyG.LockedFlagImage.enabled = false;
            adminEnemyG.LockedFlag = false;
        }
        else if(type == EnemyType.IronDog)
        {
            adminEnemyD.LockedFlagImage.enabled = false;
            adminEnemyD.LockedFlag = false;
        }
    }

    public bool TalkFalg()
    {
        var  i = true;
        if(type == EnemyType.Ghost)
        {
            i =  adminEnemy.TalkEventFlag;
        }
        else if(type == EnemyType.Golem)
        {
            i = adminEnemyG.TalkEventFlag;
        }
        else if(type == EnemyType.IronDog)
        {
            i = adminEnemyD.TalkEventFlag;
        }
        return i;
    }

    public void Reset()
    {
        if(type == EnemyType.Ghost)
        {
            adminEnemy.Reset();
        }
        else if(type == EnemyType.Golem)
        {
            adminEnemyG.Reset();
        }
        else if(type == EnemyType.IronDog)
        {
            adminEnemyD.Reset();
        }
        HP = HPStatus;
        HPSlider.value = 1;
        // NowHPRatio = HP/HPStatus;
    }
}