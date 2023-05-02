using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
	private float damage;
	void Start()
	{
		damage = GetComponentInParent<Admin_EnemyStatus>().AttackStatus;
	}


	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player")
		{
			Debug.Log("主人子にヒット");
			col.GetComponent<Move>().TakeDamage(damage);
			// col.GetComponent<CharaHP>().TakeDamage(damage);
		}
	}
}