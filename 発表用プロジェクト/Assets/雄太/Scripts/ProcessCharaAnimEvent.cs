using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessCharaAnimEvent : MonoBehaviour
{
    private Move characterScript;
    private Animator animator;
    [SerializeField]    
    private AudioClip[] sound;
    private AudioSource audioSource;
    [SerializeField]
    private GameObject weapon;
    private Admin admin;
    

    private void Start() {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        admin = GetComponentInChildren<Admin>();
        characterScript = GetComponent<Move>();
    }


    void AttackStart(AnimationEvent animationEvent) 
    {
        int SoundNumber = animationEvent.intParameter;
        audioSource.PlayOneShot(sound[SoundNumber]);
        // characterScript.Aimassist();
    }
 
    void AttackEnd() 
    {
        
    }
    void LastAttack()
    {
        animator.ResetTrigger(Admin.WeaponNumber + "Attack");
        animator.ResetTrigger(Admin.WeaponNumber + "SPAttack");
        Debug.Log("ラストアタック");
    }
 
    void StateEnd() 
    {
        characterScript.SetState(Move.MyState.Normal);
        admin.TalkEnd();
    }
    public void EndDamage() 
    {
        characterScript.SetState(Move.MyState.Normal);
    }

    public void Effect(AnimationEvent  animationEvent)
    {
        // float damageMagnification =  animationEvent.floatParameter;
        var parent = this.transform;
        // 指定したエフェクトをGameObjectとして顕現させる
        GameObject EffectObj = (Instantiate( animationEvent.objectReferenceParameter, transform.position, transform.rotation, parent) as GameObject);
        EffectObj.GetComponent<AttackByEffect>().DamageMagnification = animationEvent.floatParameter; 
    }
}
