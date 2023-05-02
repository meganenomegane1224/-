using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchEnemy : MonoBehaviour {

    [SerializeField]
    private string tagName = "Enemy";        // インスペクターで変更可能

    public static GameObject searchNearObj;         // 最も近いオブジェクト(public修飾子にすることで外部のクラスから参照できる)
    private Admin admin;
    private float lockOffDistance;

    void Start() {
        admin = GetComponentInParent<Admin>();
        // 指定したタグを持つゲームオブジェクトのうち、このゲームオブジェクトに最も近いゲームオブジェクト１つを取得
        searchNearObj = Serch();
        lockOffDistance = admin .LockOffDistance;
    }

    void Update() 
    {
        if(admin.LockOn == true && admin.LockEnemy != null)
        {
            float distance = Vector3.SqrMagnitude(admin.LockEnemy.transform.position - transform.position);

            if(distance >= lockOffDistance*lockOffDistance)
            {
                admin.LockOff();
            }
        }
        
    }

    /// <summary>
    /// 指定されたタグの中で最も近いものを取得
    /// </summary>
    /// <returns></returns>
    public GameObject Serch() 
    {

        // 最も近いオブジェクトの距離を代入するための変数
        float nearDistance = 0;

        // 検索された最も近いゲームオブジェクトを代入するための変数
        GameObject searchTargetObj = null;

        // tagNameで指定されたTagを持つ、すべてのゲームオブジェクトを配列に取得
        GameObject[] objs = GameObject.FindGameObjectsWithTag(tagName);

        // 取得したゲームオブジェクトが 0 ならnullを戻す(使用する場合にはnullでもエラーにならない処理にしておくこと)
        if (objs.Length == 0)
        {
            return searchTargetObj;
        }

        // objsから１つずつobj変数に取り出す
        foreach (GameObject obj in objs)
        {

            // objに取り出したゲームオブジェクトと、このゲームオブジェクトとの距離を計算して取得
            float distance = Vector3.Distance(obj.transform.position, transform.position);

            // nearDistanceが0(最初はこちら)、あるいはnearDistanceがdistanceよりも大きい値なら
            if (nearDistance == 0 || nearDistance > distance)
            {

                // nearDistanceを更新
                nearDistance = distance;

                // searchTargetObjを更新
                searchTargetObj = obj;
            }
        }

        //最も近かったオブジェクトを返す
        return searchTargetObj;
    }
}