using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPanelTransform : MonoBehaviour
{
    [SerializeField]
    RectTransform InventoryRect;
    [SerializeField]
    DPanelController dPanelcontroller;
    [SerializeField]
    WeaponInformation weaponInformation;

    void Start()
    {
        gameObject.SetActive(true);
        InventoryRect.localPosition = new Vector3(0,0,0);
    }


    void Update()
    {
        // if(Input.GetKeyDown(KeyCode.B))
        // {
            
        //     ChangeBagPanel();
        //     ChangeBagPanel();
        // }

        // if(Input.GetKeyDown(KeyCode.V))
        // {
        //     ChangeWeaponPanel();
        //     ChangeWeaponPanel();
        // }
    }



    public void ChangeBagPanel()
    {
        InventoryRect.localPosition = new Vector3(-1200,0,0);
    }

    public void ChangeWeaponPanel()
    {
        InventoryRect.localPosition = new Vector3(0,-700,0);
        dPanelcontroller.FlaseDPanel();
        weaponInformation.GetInformation();
    }

    public void ChangeSkillTreePanel()
    {
        InventoryRect.localPosition = new Vector3(-1200,-700,0);
    }


    public void ResetMenu()
    {
        InventoryRect.localPosition = new Vector3(0,0,0);
    }



    // [SerializeField] GameObject pausePanel;
    // [SerializeField] RectTransform pPanel;
    // private bool pushJ = true;
    // private bool pushL = false;

    // public void transPausePanelPos(){
    //     if(pausePanel.activeSelf){

    //         if(Input.GetKeyDown(KeyCode.J)&&pushJ){
    //         pPanel.Translate(-1200.0f,0f,0f);
    //         pushL = true;
    //         pushJ = false;
    //         }

    //         else if(Input.GetKeyDown(KeyCode.L)&&pushL){
    //             pPanel.Translate(1200.0f,0f,0f);
    //             pushJ = true;
    //             pushL = false;
    //         }
    //     }
    // }
}
