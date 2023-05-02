using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AppearScript : MonoBehaviour
{
	[SerializeField]
	// private GameObject[] enemy;
	// [HideInInspector]
	public Vector3 CharacterPosition;
	// [SerializeField]
	// private List<Admin_EnemyStatus> enemyStatus;
	public bool encountCharaFlag;

	public Enemy[] enemy;
	private float time;


	[System.Serializable]
	public struct Enemy
	{
		public GameObject SceneObject;
		public Admin_EnemyStatus enemyStatus;
		public bool SurviveFlag;
		public float ReSpawnCount;
		[HideInInspector]
		public float ReSpawnTime;

	}


	void Start()
	{
		for (int i = 0; i < enemy.Length; i++)
		{
			enemy[i].enemyStatus.appearNumber = i;
			enemy[i].SurviveFlag = true;
		}
	}

	public void CharacterConnect()
	{
		encountCharaFlag = true;
			foreach (var item in enemy)
			{
				if(item.SurviveFlag == true)
				{
					item.SceneObject.SetActive(true);
				}
			}
	}
	
	public void CharacterDisconnect()
	{
		encountCharaFlag = false;
		foreach (var item in enemy)
		{
			item.enemyStatus.Reset();
			item.SceneObject.SetActive(false);
		}
	}
	
	public void EnemyDie(int n)
	{
		enemy[n].SceneObject.SetActive(false);
		enemy[n].SurviveFlag = false;
		enemy[n].ReSpawnTime = Time.time + enemy[n].ReSpawnCount;

		StartCoroutine(ReSpawn(n));
	}

	IEnumerator ReSpawn(int n)
	{
		yield return new WaitForSeconds(enemy[n].ReSpawnCount);
		enemy[n].SurviveFlag = true;
		if(encountCharaFlag == true)
		{
			enemy[n].SceneObject.SetActive(true);
		}
	}
}