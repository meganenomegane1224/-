using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Admin_Auto : MonoBehaviour
{
    [SerializeField]
    private CharaState state;            //キャラの状態を定義
    [SerializeField]
    private float HPStatus = 100;        //きゃらのHPを設定
    [SerializeField]
    private float AttackStatus = 10;     //キャラの攻撃力を設定

    public float hp;　　　　　　　　　　　//HPを設定
    
    [SerializeField]
    public Transform enemyTransform;    //攻撃対象の敵の位置を設定
    private Animator animator;          //アタッチ
    private CharacterController characterController;//アタッチ
    private AudioSource audioSource;//アタッチ

    [SerializeField]
    private Admin_Effect[] admin_Effects;//エフェクトを配列化して格納
    [SerializeField]
    private AudioClip[] audioClips;//効果音を適用
    [SerializeField]
    public Slider hpSlider;        //HPスライダーを適用
    [SerializeField]
    private ReviveArea reviveArea;//復活エリアのprefabをアタッチ


    const float ROTATE_SPEED = 600f;
    const float MOVE_TO_ATTACK_DISTANCE =4;



    private enum CharaState//キャラの状態を定義
    {
        Idle,
        Move,
        Attack,
        Avoidance,
        Damage,
        HighAttack,
        Down
    }
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();//取得
        characterController = GetComponent<CharacterController>();//取得
        audioSource = GetComponent<AudioSource>();//取得
        SetState(CharaState.Idle);//ノーマル状態にする
        hp = HPStatus;//Hpを設定
        hpSlider.value = 1;//値を設定
    }

    // Update is called once per frame
    void Update()
    {
        if(state == CharaState.Down)return;//ダウン状態だと実行しない

        var distance = Vector3.SqrMagnitude(transform.position - enemyTransform.position);//敵との距離を計算
        Quaternion look = new Quaternion(0,0,0,0);//回転値を定義
        if(distance > MOVE_TO_ATTACK_DISTANCE)//距離が離れていたら移動状態にする
        {
            if(state != CharaState.Move)SetState(CharaState.Move);
            
        }
        else if (distance <= MOVE_TO_ATTACK_DISTANCE && (state == CharaState.Idle || state == CharaState.Move))//距離が近かったら攻撃する
        {
            SetState(CharaState.Attack);
        }

        if(state == CharaState.Move || state == CharaState.Attack)//攻撃、移動状態の時は敵の方向を向く
        {
            look = Quaternion.LookRotation(enemyTransform.position -transform.position);
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation , look ,ROTATE_SPEED*Time.deltaTime);//回転を実行する
    }

    private void SetState(CharaState tempState)
    {
        var PostState = state;
        state = tempState;
        if(tempState == CharaState.Idle)//idol状態なら攻撃を中止し、停止する
        {
            animator.ResetTrigger("Attack");
            animator.SetFloat("Speed" , 0);
        }
        else if(tempState ==  CharaState.Move)//移動状態なら移動アニメーションを再生する
        {
            animator.SetFloat("Speed" , 1);
        }
        else if(tempState == CharaState.Attack)//攻撃状態なら止まって、攻撃アニメーションを再生する
        {
            animator.SetTrigger("Attack");
            animator.SetFloat("Speed" , 0);
        }
        else if(tempState == CharaState.Damage)//ダメージアニメーションを再生する
        {
            animator.Play("Damage");
        }
        else if(tempState == CharaState.Avoidance)//回避状態なら、過去状態がmoveなら前方回避、それ以外なら後方回避する
        {
            if(PostState == CharaState.Move)
            {
                animator.Play("前方回避");
            }
            else
            {
                animator.Play("後方回避");
            }
        }
        else if(tempState == CharaState.Down)//死亡モーションを流す、攻撃をやめる、移動をやめる
        {
            animator.SetTrigger("Die");
            animator.ResetTrigger("Attack");
            animator.SetFloat("Speed" , 0);
        }
    }
    //ダメージを受けたときに呼ばれる関数
    public void TakeDamage(float damage)
    {
        if(state ==CharaState.Down)return;//ダウン状態なら実行しない

        if(Random.Range(0,4) == 0)//確率で回避し、ジャスト回避攻撃を実行する
        {
            SetState(CharaState.Avoidance);
            return;
        }
        hp -= damage;//HPを計算する
        hpSlider.value = hp/HPStatus;//HPバーを更新

        SetState(CharaState.Damage);//ダメージ状態にする

        if(hp <= 0)//HPが０以下ならダウン状態にし、蘇生エリアを出現させる
        {
            SetState(CharaState.Down);
            Instantiate(reviveArea,transform.position,transform.rotation,transform);
        }
    }

    //蘇生に成功したときに呼ばれる関数
    public void Revive()
    {
        SetState(CharaState.Idle);//idolにする
        hp = HPStatus;
        hpSlider.value = 1;//HPを満タンにする
    }
    public void StateEnd()
    {
        if(state == CharaState.Down)return;//ダウン状態なら実行しない
        SetState(CharaState.Idle);//アイドル状態にする
    }
    public void EffectOff()
    {

    }
    public void EffectOn()
    {

    }
    public void FootSound()
    {

    }
    public void AttackStart()
    {

    }
}
