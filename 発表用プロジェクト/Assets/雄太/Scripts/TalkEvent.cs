using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;
using UnityEngine.Animations.Rigging;

public class TalkEvent : MonoBehaviour
{
    [SerializeField]
    public TalkStartButton talkStartButton;
    [SerializeField]
    private Admin admin;
    [SerializeField]
    private int talkNumber;
    private GameObject EnemyObj;
    private Vector3 velocity;
    private Quaternion targetRotation;
    private GameObject TargetObj;
    [SerializeField]
    private Vector3 FirstVelocity;
    [SerializeField]
    private Item item;


    // Start is called before the first frame update
    void Start()
    {
        if(item == null)
        {
            EnemyObj = transform.parent.gameObject;
            talkStartButton.parentObj = this;
        }
        if(admin == null)
        {
            admin = GameObject.Find("管理").GetComponent<Admin>();
        }
    }
    
    void Update()
    {
        if(item == null)
        {
            EnemyObj.transform.rotation = Quaternion.RotateTowards(EnemyObj.transform.rotation , targetRotation , 300*Time.deltaTime);
        }
    }
    void OnTriggerStay(Collider col)
    {
        if(col.tag == "Player" && admin.talkObject.Contains(this) == false)
        {
            admin.InTalkRange(this);
            
            TargetObj = col.gameObject;
        }
    }
    void OnTriggerExit(Collider col)
    {
        if(col.tag == "Player" && admin.talkObject.Contains(this) == true)
        {
            admin.OutTalkRange(this);
            TargetObj = null;
        }
    }

    public void TalkStart()
    {
        if(item == null)
        {
            admin.Talk(talkNumber , this);
            velocity = -EnemyObj.transform.position + TargetObj.transform.position;
            targetRotation = Quaternion.LookRotation(new Vector3(velocity.x , 0 , velocity.z));
        }
        else
        {
            item.GetItem();
            admin.OutTalkRange(this);
        }
        
    }

    public void TalkOff()
    {
        velocity = Vector3.zero;
        targetRotation = Quaternion.LookRotation(FirstVelocity);
    }
}