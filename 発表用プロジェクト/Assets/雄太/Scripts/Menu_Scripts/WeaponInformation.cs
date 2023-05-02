using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInformation : MonoBehaviour
{
    [SerializeField]
    GameObject[] WeaponUI;
    GameObject UI_mainWeapon;
    GameObject UI_subWeapon;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetInformation()
    {
        if(UI_mainWeapon && UI_mainWeapon != null)
        {
            Destroy(UI_mainWeapon);
            Destroy(UI_subWeapon);

        }
       
        var parent = this.transform;
        UI_mainWeapon = Instantiate(WeaponUI[Admin.MainWeapon], new Vector3(790 , 385 , 0) , transform.rotation , parent);
        UI_subWeapon = Instantiate(WeaponUI[Admin.SubWeapon] , new Vector3(790 , 160 , 0), transform.rotation , parent);
    }
}
