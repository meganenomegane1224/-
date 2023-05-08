using UnityEngine;
using UnityEngine.UI;

public class TalkStartButton : MonoBehaviour
{
    [SerializeField]
    private string ButtonText = "話す";     //takjButtonのテキストの内容を設定
    // [HideInInspector]
    public TalkEvent parentObj;            //親にあるTalkButtonを定義。この変数はTalkEventによって自動的に定義される
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<Text>().text = ButtonText; //子オブジェクトのＴｅｘｔを探し、textの内容を変更
    }

    public void OnClick()
    {
        parentObj.TalkStart();    //ボタンが押されたとき、親オブジェクトにあるTalkEventの会話開始プログラムを作動させる
    }
}