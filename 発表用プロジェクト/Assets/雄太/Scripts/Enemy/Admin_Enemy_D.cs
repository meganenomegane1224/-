using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class Admin_Enemy_D : MonoBehaviour
{
    public EnemyState state;

    [Header("行動設定")]
        [SerializeField , Tooltip("フリーズ状態の時の最大待機時間")]
        private float MaxWaitTime = 1;
        [SerializeField , Tooltip("Idle状態の時の最大移動範囲")]
        private float MaxIdleMoveRange;
        [SerializeField , Tooltip("Idle状態の時の最小移動範囲")]
        private float MinIdleMoveRange = 0;
        [SerializeField , Tooltip("キャラの行動起点　この位置を中心に行動半径を決める")]
        private Transform LandmarkTransform;
        [SerializeField , Tooltip("ロック状態に表示する画像")]
        public Image LockedFlagImage;
        [SerializeField , Tooltip("敵が離れすぎたときに強制的に位置をリセットする距離")]
        private float BackDistance = 20;
        [SerializeField]
        private bool isIdleMove = true;

    [Space(8)]

    [Header("攻撃設定")]
        [SerializeField , Tooltip("Standby状態から攻撃状態に移行するまでの時間　この時間が短いほど攻撃頻度が高い")]
        private float AttackCountdown = 5;
        [SerializeField , Tooltip("キャラの攻撃種類の数")]
        private int AttackTypeCount = 2;
        [SerializeField , Tooltip("無敵状態が終了するまでの時間　　無敵状態はキャラがダメージモーションをしなくなる状態　ダメージは受ける")]
        private float invincibleOffTime = 10;
        [SerializeField , Tooltip("無敵状態をオンにするのに必要なダメージ判定の回数")]
        private int invincibleOnCount = 10; 
        [SerializeField , Tooltip("主人公と接敵状態になる距離　　この距離が長いほど敵が主人子を見つけやすい")]
        private float EncountDistance;
    
    [Space(8)]

    [Header("状態表示")]
        public bool encounterBool;
        public bool LockedFlag;       
        public bool invincibleFlag;   
        public bool TalkEventFlag;
        [SerializeField]
        private bool moveBool; 
        [SerializeField]
        private float elapsedTime; 
        public Vector3 destination; 
        
    [Space(8)] 
    
    [SerializeField]
    private Eff_DamageArea eff_DamageArea;
    [SerializeField]
    private Admin_EnemyEffect[] Effect;   
    private Quaternion targetRotation;
    private Animator animator;
    private Admin_EnemyStatus enemyStatus;
    private CharacterController characterController;
    private AppearScript appearScript;
    private float AttackCount;     
    private Vector3 LookPosition;      
    private int invinciblecount;  
    private Vector3 velocity;    
    private float StandbyMoveSpeed;
    private float moveSpeed;
    private float NowMoveSpeed;    
    private int animSpeedHash;   
    private Vector3 StartPosition;
    private float distance;


    public enum EnemyState
    {
        Idle,
        Standby,
        Wait,
        Move,
        Damage,
        Attack,
        HighAttack,
        Die,
        TalkEvent,
        none,
    }

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();GetComponent<Animator>();
        animator = GetComponent<Animator>();
        enemyStatus = GetComponent<Admin_EnemyStatus>();
        appearScript = GetComponentInParent<AppearScript>();
        if(GetComponentInChildren<TalkEvent>())
        {
            TalkEventFlag =true;
        }
        else
        {
            TalkEventFlag = false;
        }
        moveSpeed = enemyStatus.MoveSpeed;
        NowMoveSpeed = moveSpeed;
        StandbyMoveSpeed = moveSpeed /4;
        state = EnemyState.none;
        SetState(EnemyState.Idle);
        elapsedTime = 0 - Random.Range(2 , MaxWaitTime);
        encounterBool = false;
        if(LandmarkTransform == null)
        {
            LandmarkTransform = this.transform;
        }
        LockedFlagImage.enabled = false;
        LockedFlag = false;
        invincibleFlag = false;
        invinciblecount = 0;
        animSpeedHash = Animator.StringToHash("Speed");
        StartPosition = transform.position;
        enemyStatus.HPSlider.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        
        if(state == EnemyState.Wait)
        {
            elapsedTime += Time.deltaTime;
            if(elapsedTime>0)
            {
                DecideNextState();
            }
            
        }

        if(encounterBool == true && state != EnemyState.HighAttack)
        {
            LookPosition = new Vector3(appearScript.CharacterPosition.x , transform.position.y , appearScript.CharacterPosition.z);
        }

        if(appearScript.CharacterPosition != null)
        {
            distance = Vector3.SqrMagnitude(transform.position - appearScript.CharacterPosition);
        }
       
        if(state == EnemyState.Move)
        {
            destination = new Vector3(appearScript.CharacterPosition.x , transform.position.y , appearScript.CharacterPosition.z);
            if(distance > 80)
            {
                SetState(EnemyState.HighAttack);
            }
        }

        if(state == EnemyState.Standby)
        {
            AttackCount += Time.deltaTime;
            if(AttackCount>0)
            {                
                SetState(EnemyState.Attack);
            }

            
            // else if(distance > 100)
            // {
            //     SetState(EnemyState.Attack);
            // }
        }
        
        if(moveBool == true)
        {
            velocity = (destination- transform.position).normalized * NowMoveSpeed; 
            var lookposi =  new Vector3(LookPosition.x , transform.position.y , LookPosition.z) - transform.position;
            targetRotation = Quaternion.LookRotation(lookposi);
            characterController.Move(velocity*Time.deltaTime);
            animator.SetFloat(animSpeedHash, velocity.magnitude  + 1 , 0.2f , Time.deltaTime);

            if(Vector3.SqrMagnitude(transform.position - destination) <0.5f && state != EnemyState.Move)  
            {
                moveBool = false;
                velocity = Vector3.zero;
                if(state != EnemyState.HighAttack)
                {
                    SetState(EnemyState.Wait);
                }
                
            }
            if(state == EnemyState.Move && distance < 4)
            {
                moveBool = false;
                velocity = Vector3.zero;
                SetState(EnemyState.Wait);
            }
        }
        else
        {
            animator.SetFloat(animSpeedHash ,  Mathf.Lerp(animator.GetFloat(animSpeedHash) , -0.5f , Time.deltaTime));
        }

        if(TalkEventFlag == false && state != EnemyState.Die)
        {
            if(encounterBool == false && distance < EncountDistance*EncountDistance)
            {
                EncounterCharactor();
            }
            if(encounterBool == true && distance > EncountDistance*EncountDistance)
            {
                LostCharactor();
            }
        }

        if(Vector3.SqrMagnitude(transform.position - LandmarkTransform.position) > BackDistance*BackDistance)
        {
            gameObject.SetActive(false);
            Reset();
            gameObject.SetActive(true);
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation , targetRotation , 300*Time.deltaTime);
    }

    public void SetState(EnemyState tempState)
    {
        if(TalkEventFlag == false)
        {
            if(state == EnemyState.Die) return;
            var PostState = state;
            state = tempState;
            if(tempState == EnemyState.Wait)
            {
                moveBool = false;
                if(invincibleFlag == true)
                {
                    elapsedTime = 0;
                }
                else if(PostState == EnemyState.Idle)
                {
                    elapsedTime = 0 - Random.Range(2 , MaxWaitTime);
                }
                else if(PostState == EnemyState.Standby)
                {
                    elapsedTime = 0 - Random.Range(0,1);
                }
                else if(PostState == EnemyState.Move)
                {
                    elapsedTime = 0;
                }
                else if(PostState == EnemyState.Attack)
                {
                    elapsedTime = 0 - Random.Range(2 , MaxWaitTime);
                }
                else if(PostState == EnemyState.Damage)
                {
                    elapsedTime = -0.1f;
                }
                else 
                {
                    elapsedTime = -4;
                }
            }
            else if(tempState == EnemyState.Idle)
            {
                if(isIdleMove == true)
                {
                    moveBool = true;
                    destination = new Vector3(LandmarkTransform.position.x + Random.Range(MinIdleMoveRange,MaxIdleMoveRange),transform.position.y,LandmarkTransform.position.z + Random.Range(MinIdleMoveRange,MaxIdleMoveRange));
                    LookPosition = destination;
                    NowMoveSpeed = moveSpeed;
                }
                else if(Vector3.SqrMagnitude(transform.position - LandmarkTransform.position) > 1)
                {
                    moveBool = true;
                    destination = new Vector3(LandmarkTransform.position.x + Random.Range(-0.5f , 0.5f),transform.position.y,LandmarkTransform.position.z + Random.Range(-0.5f , 0.5f));
                    LookPosition = destination;
                    NowMoveSpeed = moveSpeed;
                }
                else
                {
                    moveBool = false;
                    animator.SetFloat("Speed" , 0);
                }
                
            }
            else if(tempState == EnemyState.Standby)
            {
                moveBool = true;
                destination = new Vector3(appearScript.CharacterPosition.x + Random.Range(-5,5) , transform.position.y ,appearScript.CharacterPosition.z + Random.Range(-5,5));
                NowMoveSpeed = StandbyMoveSpeed;
            }
            else if(tempState == EnemyState.Move)
            {
                
                moveBool =true;
                destination = new Vector3(appearScript.CharacterPosition.x , transform.position.y , appearScript.CharacterPosition.z);
                NowMoveSpeed = moveSpeed;
            }
            else if(tempState == EnemyState.Attack)
            {
                velocity = Vector3.zero;       
                moveBool = false;         
                var lookposi = new Vector3(LookPosition.x , transform.position.y , LookPosition.z) - transform.position;
                targetRotation = Quaternion.LookRotation(lookposi);
                animator.SetTrigger("Attack"+ Random.Range(1 , AttackTypeCount+1));
                Debug.Log("こうげーーーーーーーーーーーーーーき");
                if(invincibleFlag == false)
                {
                    AttackCount = 0-AttackCountdown;
                }
                else
                {
                    AttackCount = -AttackCountdown-2; 
                }
            }
            else if(tempState == EnemyState.HighAttack)
            {
                animator.SetTrigger("Attack4");
                moveBool = false;
                // eff_DamageArea.SetDamageArea(appearScript.CharacterPosition);
                // targetRotation = Quaternion.LookRotation(appearScript.CharacterPosition);
            }
            else if(tempState == EnemyState.Damage)
            {
                if(PostState == EnemyState.Attack)
                {
                    state = EnemyState.Attack;
                    return;
                }
                else if(PostState == EnemyState.HighAttack)
                {
                    state = EnemyState.HighAttack;
                    return;
                }
                moveBool = false;
                velocity = Vector3.zero;
                ResetEffect();
                invinciblecount ++;
                if(invinciblecount >= invincibleOnCount)
                {
                    invincibleFlag = true;
                    StartCoroutine(invincibleOff());
                }

                if(PostState == EnemyState.Damage)
                {
                    animator.SetTrigger("Damage2");
                }
                else
                {
                    animator.Play("Damage1");
                }
            }
            else if(tempState == EnemyState.Die)
            {
                ResetEffect();
                invincibleFlag = false;
                encounterBool = false;
                velocity = Vector3.zero;
                moveBool = false;
                animator.SetTrigger("Die");
                Debug.Log("こいつしんだぁ");
            }
        }
    }

    public void DecideNextState()
    {
        if(encounterBool == true && distance<= 12)
        {
            SetState(EnemyState.Standby);
        }
        else if(encounterBool == true && distance > 12)
        {
            SetState(EnemyState.Move);
        }
        else
        {
            SetState(EnemyState.Idle);
        }
    }

    public void EncounterCharactor()
    {
        encounterBool = true;
        DecideNextState();
        AttackCount = 0-AttackCountdown;
        enemyStatus.HPSlider.gameObject.SetActive(true);
    }

    public void LostCharactor()
    {
        state = EnemyState.none;
        SetState(EnemyState.Wait);
        invincibleFlag = false;
        encounterBool = false;
        enemyStatus.HPSlider.gameObject.SetActive(false);
    }

    private IEnumerator invincibleOff()
    {
        var time = new WaitForSeconds(invincibleOffTime);
        yield return  time;
        invincibleFlag = false;
        invinciblecount = 0;
    }

    // 以下、アニメーションイベント等の受動メソッド

    public void StateEnd()
    {
        SetState(EnemyState.Wait);
    }

    public void AttackOn(AnimationEvent animationEvent)
    {
        
        if(animationEvent.intParameter == 4)
        {
            if(distance > 144)
            {
                
                Instantiate(eff_DamageArea ,transform.position , Quaternion.identity);
                // eff_DamageArea.SetDamageArea(transform.position);
            }
            else
            {
                Instantiate(eff_DamageArea , new Vector3(appearScript.CharacterPosition.x , appearScript.CharacterPosition.y + 0.1f , appearScript.CharacterPosition.z) , Quaternion.identity);
                // eff_DamageArea.SetDamageArea(appearScript.CharacterPosition);
                moveBool = true;
                destination = appearScript.CharacterPosition;
                NowMoveSpeed = 7;
            }
            
            // else
            // {
            //     destination = appearScript.CharacterPosition;
            // }
            
        }
        else
        {
            Effect[animationEvent.intParameter].gameObject.SetActive(true);
            Effect[animationEvent.intParameter].EffectStart((int)animationEvent.floatParameter);
        }
    }
    public void AttackOff(int n)
    {
        // if(n == 4)
        // {
        //     // eff_DamageArea.EndDamageArea();
        //     Destroy(eff_DamageArea);
        // }
        if(n != 4)
        {
            Effect[n].EffectEnd();
            Effect[n].gameObject.SetActive(false);
        }
        
    }

    public void ResetEffect()
    {
        foreach (var item in Effect)
        {
            if(item.gameObject.activeSelf == true)
            {
                item.EffectEnd();
                item.gameObject.SetActive(false);
            }
            
        }
    }

    public void Reset()
    {
        Debug.Log("りせっとぉ");
        ResetEffect();
        transform.position = StartPosition;
        AttackCount = 0-AttackCountdown;
        elapsedTime = -4;
        encounterBool = false;
        LockedFlagImage.enabled = false;
        LockedFlag = false;
        invinciblecount = 0;
        invincibleFlag = false;
        moveBool = false;
        state = EnemyState.none;
        SetState(EnemyState.Idle);
    }
}