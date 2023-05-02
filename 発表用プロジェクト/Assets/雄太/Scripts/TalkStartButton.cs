using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkStartButton : MonoBehaviour
{
    [SerializeField]
    private Text text;
    [SerializeField]
    private string ButtonText = "話す";
    [HideInInspector]
    public TalkEvent parentObj;
    // Start is called before the first frame update
    void Start()
    {
        text.text = ButtonText;
    }

    public void OnClick()
    {
        parentObj.TalkStart();
    }
}