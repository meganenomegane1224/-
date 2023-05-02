using System.Collections;
using System.Collections.Generic;


using UnityEngine.UI;
using UnityEngine;

public class DPanelController : MonoBehaviour
{
    RectTransform rect;
    private int WeaponNumber;
    [SerializeField]
    Admin admin;
    [SerializeField]
    WeaponInformation weaponInformation;



    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }


    public void TrueDPanel(int number)
    {
        gameObject.SetActive(true);
        WeaponNumber = number;
    }

    public void FlaseDPanel()
    {
        gameObject.SetActive(false);
        weaponInformation.GetInformation();
    }



    public void ChangeMainWeapon()
    {
        if(Admin.MainWeapon != WeaponNumber)
        {
            admin.ChangeMainWeapon(WeaponNumber);
        }
        
    }

    public void ChangeSubWeapon()
    {
        if(Admin.SubWeapon != WeaponNumber)
        {
            admin.ChangeSubWeapon(WeaponNumber);
        }
        
    }
}
