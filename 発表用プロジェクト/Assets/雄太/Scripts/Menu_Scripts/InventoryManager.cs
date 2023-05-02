using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] GameObject inventory;

   private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
               //初期状態は非表示
    }

    // private void itemPanelTransfom(){
    //     itemPanelPos = inventory.Transform.localPosition;
    //     transform.translate 
        
    // }

    public void Toggle1(){

        inventory.SetActive(!inventory.activeSelf);

        if (inventory.activeSelf)
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        else{
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

        }
    }
}
