using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MenuController : MonoBehaviour
{
    RectTransform rect;
    [SerializeField]
    GameObject DecidePanel;

    void Start() 
    {
	    rect = GetComponent < RectTransform > ();
        rect.localPosition = new Vector3(1000,0,0);
    }


    public void SlidMenu()
    {
        rect.localPosition = new Vector3(0, 0, 0);
        
    }

    public void ResetMenu()
    {
        rect.localPosition = new Vector3(1000,0,0);
        DecidePanel.SetActive(false);
    }


}
