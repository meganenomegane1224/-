using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Admi_Setting : MonoBehaviour
{
    private void Awake() 
    {
        QualitySettings.vSyncCount = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeFPS(int n)
    {
        Application.targetFrameRate = n;
    }
}
