using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlaying : MonoBehaviour
{

    [SerializeField]
    private int ThisNumber;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void Apper(int number)
    {
        if(number == ThisNumber)
        {
            this.enabled = true;
        }
        else
        {
            this.enabled = false;
        }
    }
}
