using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class New_EnemyMove : MonoBehaviour
{
    public bool moveBool;
    private Admin_Enemy adminEnemy;
    private CharacterController characterController;
    public Vector3 velocity;
    private Animator animator;
    private float moveSpeed;
    private float StandbyMoveSpeed;
    private float NowMoveSpeed;
    private Admin_EnemyStatus enemyStatus;

    // Start is called before the first frame update
    void Start()
    {
        adminEnemy = GetComponent<Admin_Enemy>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        enemyStatus = GetComponent<Admin_EnemyStatus>();
        moveBool = false;
        velocity = Vector3.zero;
        moveSpeed = enemyStatus.MoveSpeed;
        NowMoveSpeed = moveSpeed;
        StandbyMoveSpeed = moveSpeed / 4;
    }

    // Update is called once per frame
    void Update()
    {
        if(moveBool == true)
        {
            velocity = Vector3.zero;
            // animator.SetFloat("Speed" , 1.0f);
            // velocity = (adminEnemy.GetDestination() - transform.position).normalized * NowMoveSpeed;
            // transform.LookAt(new Vector3(adminEnemy.GetLookPosition().x , transform.position.y , adminEnemy.GetLookPosition().z));

        }
        // if(Vector3.Distance(transform.position , adminEnemy.GetDestination()) <0.7f && moveBool == true) 
        // {
        //     moveBool = false;
        //     velocity = Vector3.zero;
        //     adminEnemy.SetState(Admin_Enemy.EnemyState.Wait);
        //     // animator.SetFloat("Speed" , 0.0f);
        // }
        characterController.Move(velocity * Time.deltaTime);
        animator.SetFloat("Speed" , Mathf.Lerp( animator.GetFloat("Speed") , velocity.magnitude , Time.deltaTime));
        Debug.Log(velocity.magnitude);


        if(adminEnemy.state == Admin_Enemy.EnemyState.Standby)
        {
            NowMoveSpeed = StandbyMoveSpeed;
        }
        else 
        {
            NowMoveSpeed = moveSpeed;
        }


        // if(Input.GetKeyDown("p"))
        // {
        //     animator.SetTrigger("Attack1");
        // }
        // if(Input.GetKeyDown("o"))
        // {
        //     animator.SetTrigger("Attack2");
        // }
    }

    public void Attack(int n)
    {
        animator.SetTrigger("Attack" + n);
    }
    public void Damage()
    {
        if(adminEnemy.state == Admin_Enemy.EnemyState.Damage && adminEnemy.invincibleFlag == false)
        {
            animator.SetTrigger("Damage2");
        }
        else if(adminEnemy.invincibleFlag == false)
        {
            Debug.Log("だめーーーーーーーじ");
            moveBool = false;
            animator.SetTrigger("Damage");
            adminEnemy.SetState(Admin_Enemy.EnemyState.Damage);
        }
        else if (adminEnemy.invincibleFlag == true && adminEnemy.state != Admin_Enemy.EnemyState.Attack)
        {
            adminEnemy.DecideNextState();
        }
    }
    public void Die()
    {
        animator.SetTrigger("Die");
    }

}
