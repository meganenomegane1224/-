using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendTransform : MonoBehaviour
{
    [SerializeField]
    private Admin_Auto admin_Auto;//自動戦闘させたいキャラについているadmin_autoを定義する
    // Update is called once per frame
    void Update()
    {
        admin_Auto.enemyTransform = this.transform;  //admin_autoにこの王bジェクトの座標をリアルタイムで送信する
    }
}
