using UnityEngine;

public class ChangeEquip : MonoBehaviour
{

    [SerializeField]
    private GameObject[] weapons;//装備可能な武器を子オブジェクトに入れ、それらを格納する
    private int equipment = 1;　//装備している武器番号を格納
    [SerializeField]
    private Admin admin;       //adminをアタッチ

    // Use this for initialization
    void Start()
    {
        ChangeWeapon();//武器変更を実行s
    }
    // Update is called once per frame
    void Update()
    {
        if(admin.isWorkeingMobileImage() == false && Input.GetKeyDown("e"))
        {
            ChangeWeapon();//PCで動作していて、eキーが押されたとき、武器変更を実行
        }
    }

    // サブ武器ならメイン武器に、メイン武器ならサブ武器に武器を変更する
    public void ChangeWeapon() 
    {
        if(equipment == Admin.MainWeapon)
        {
            weapons[equipment].SetActive(false);
            equipment = Admin.SubWeapon;
            weapons[equipment].SetActive(true);
        }
        else
        {
            weapons[equipment].SetActive(false);
            equipment = Admin.MainWeapon;
            weapons[equipment].SetActive(true);
        }

        Admin.WeaponNumber = equipment;//adminの武器ナンバーを更新
        admin.ReturnAccess();//アニメーションのリセットを実行
    }
}