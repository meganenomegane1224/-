using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class New_ProcesEnemyAnimEvent : MonoBehaviour
{
    private Admin_Enemy adminEnemy;
    private GameObject AttackObj;
    // Start is called before the first frame update
    void Start()
    {
        adminEnemy = GetComponent<Admin_Enemy>();
    }

    public void StateEnd()
    {
        adminEnemy.SetState(Admin_Enemy.EnemyState.Wait);
    }

    public void AttackOn(AnimationEvent animationEvent)
    {
        var parent = this.transform;
        AttackObj = (Instantiate(animationEvent.objectReferenceParameter , transform.position , transform.rotation, parent)as GameObject);
    }
    public void AttackOff(int n)
    {
        Destroy(AttackObj);
    }

    public void Delete()
    {
        
    }
}
