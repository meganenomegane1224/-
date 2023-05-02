using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTransform : MonoBehaviour
{
    [SerializeField] GameObject itemPanel;
    [SerializeField] RectTransform iPanel;

    private bool pushJ = true;
    private bool pushL = false;

    public void transItemPanelPos(){
        if(itemPanel.activeSelf){

        if(Input.GetKeyDown(KeyCode.J)&&pushJ){
            iPanel.Translate(-1000f,0f,0f);
            pushL = true;
            pushJ= false;
            }

        else if(Input.GetKeyDown(KeyCode.L)&&pushL){
                Debug.Log("pushL");
                iPanel.Translate(1000f,0f,0f);
                pushJ = true;
                pushL = false;
            }
        }
    }
}
