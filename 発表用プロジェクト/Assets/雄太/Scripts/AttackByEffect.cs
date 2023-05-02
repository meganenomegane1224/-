
using UnityEngine;
using System.Collections;
 
public class AttackByEffect : MonoBehaviour 
{

	public float DamageMagnification;
	private float damage;
	[SerializeField]
	private bool EffectDamageFlag = true;
	[SerializeField]
	private bool Damagecontinue  = false;
	[SerializeField]
	private float DamageInterval;
	private float currentTime;

	void OnTriggerEnter(Collider col) 
	{
		if(EffectDamageFlag == true && col.tag == "Enemy")
		{
			if(Damagecontinue == false )
			{
				damage = Admin.AttackStatus * DamageMagnification;
				Debug.Log("敵にヒット");
				// col.GetComponent<MoveEnemy>().TakeDamage();
				// col.GetComponent<HP>().TakeDamage(damage);
				col.GetComponent<Admin_EnemyStatus>().TakeDamage(damage);
				//col.GetComponent<CconEnemy>().SetState(CconEnemy.MobEnemyState.Damage);

			}
			else
			{
				Debug.Log("多段ヒット");
				// col.GetComponent<MoveEnemy>().TakeDamage();
			}
		}
		
	}

	void OnTriggerStay(Collider col)
	{
		if(EffectDamageFlag == true && col.tag == "Enemy" &&  Damagecontinue == true)
		{
			currentTime += Time.deltaTime;
			damage = Admin.AttackStatus * DamageMagnification;

			if(currentTime > DamageInterval)
			{
				// col.GetComponent<HP>().TakeDamage(damage);
				// col.GetComponent<MoveEnemy>().TakeDamage();
				col.GetComponent<Admin_EnemyStatus>().TakeDamage(damage);
				currentTime = 0f;
			} 
		}
		
	}

}
