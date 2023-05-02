using UnityEngine;

public class ChangeEquip : MonoBehaviour
{

    [SerializeField]
    private GameObject[] weapons;
    private int equipment = 1;
    [SerializeField]
    private Admin admin;

    // Use this for initialization
    void Start()
    {
        ChangeWeapon();
    }
    // Update is called once per frame
    void Update()
    {
        if(admin.isWorkeingMobileImage() == false && Input.GetKeyDown("e"))
        {
            ChangeWeapon();
        }
    }

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

        Admin.WeaponNumber = equipment;
        // Admin_Date.SaveDateOther(3);
        admin.ReturnAccess();
    }
}