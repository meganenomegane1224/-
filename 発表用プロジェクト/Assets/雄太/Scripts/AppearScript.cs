using UnityEngine;
using System.Collections;

public class AppearScript : MonoBehaviour
{
	public Vector3 CharacterPosition;        //主人公のtransformを格納
	public bool encountCharaFlag;            //主人公が敵発生範囲にいるかどうか

	public Enemy[] enemy;                    //出現させる敵の情報を配列化して格納

	[System.Serializable]
	public struct Enemy                      
	{
		public GameObject SceneObject;      //敵オブジェクトを格納
		public Admin_EnemyStatus enemyStatus;//上に格納した敵のadmin_enemyStatusを格納
		public bool SurviveFlag;             //敵が生きているかどうか
		public float ReSpawnCount;           //その敵が復活するのに必要な時間
		[HideInInspector]
		public float ReSpawnTime;            //復活までの時間を定義
	}


	void Start()
	{
		// それぞれのenemyStatusに番号を割り振る　死亡判定に利用
		for (int i = 0; i < enemy.Length; i++)
		{
			enemy[i].enemyStatus.appearNumber = i;
			enemy[i].SurviveFlag = true;
		}
	}

	// キャラクターの敵発生範囲内に入ったときに呼ばれる関数
	public void CharacterConnect()
	{
		encountCharaFlag = true;
			foreach (var item in enemy)//すべての敵を出現させる
			{
				if(item.SurviveFlag == true)
				{
					item.SceneObject.SetActive(true);
				}
			}
	}
	
	// キャラクターの敵発生範囲外になったときに呼ばれる関数
	public void CharacterDisconnect()
	{
		encountCharaFlag = false;
		foreach (var item in enemy)//すべての敵を非表示にする
		{
			item.enemyStatus.Reset();//状態をリセットする
			item.SceneObject.SetActive(false);
		}
	}
	
	// 敵が死んだときに呼ばれる関数
	public void EnemyDie(int n)
	{
		enemy[n].SceneObject.SetActive(false);//敵を非表示にする
		enemy[n].SurviveFlag = false;
		enemy[n].ReSpawnTime = Time.time + enemy[n].ReSpawnCount;//復活時間を設定

		StartCoroutine(ReSpawn(n));//復活を実行するコルーチンを実行
	}

	IEnumerator ReSpawn(int n)
	{
		yield return new WaitForSeconds(enemy[n].ReSpawnCount);//復活時間分だけ待つ　時間が経過したら敵を復活させる
		enemy[n].SurviveFlag = true;
		if(encountCharaFlag == true)//エンカウント状態の時のみ表示する
		{
			enemy[n].SceneObject.SetActive(true);
		}
	}
}