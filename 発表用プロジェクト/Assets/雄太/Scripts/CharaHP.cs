using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharaHP : MonoBehaviour
{
    
    private  float charamaxHp;	// 敵キャラのHP最大値を100とする
    private float charahp;  //HP
    [SerializeField]
    private Slider slider;	// シーンに配置したSlider格納用
    private float DefencePoint;
    Move move;


    void Start()
    {
        move= GetComponent<Move>();
        charamaxHp = Admin.HPStatus;
        slider.maxValue = charamaxHp;    // Sliderの最大値を敵キャラのHP最大値と合わせる
        charahp = charamaxHp;      // 初期状態はHP満タン
        slider.value = charahp;	// Sliderの初期状態を設定（HP満タン）
        DefencePoint = Admin.DefenceStatus;
    }


    public void TakeDamage(float damage)
    {
        if(move.state != Move.MyState.Avoidance)
        {
            charahp -= damage * (1 - DefencePoint);
            slider.value = charahp;	// Sliderに現在HPを適用
            Debug.Log("主人公HP = " + slider.value);
        }



        if (slider.value <= 0)
        {
            Debug.Log("死亡");
        }
    }

    public void LevelUP()
    {
        charamaxHp = Admin.HPStatus;
        slider.maxValue = charamaxHp;    // Sliderの最大値を敵キャラのHP最大値と合わせる
        charahp = charamaxHp;      // 初期状態はHP満タン
        slider.value = charahp;	// Sliderの初期状態を設定（HP満タン）
        Debug.Log("レベルアップ　現在HPは"　+ charamaxHp);
    }
}
