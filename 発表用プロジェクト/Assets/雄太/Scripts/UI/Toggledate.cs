using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Toggledate : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private RectTransform handle;
    private Admin_UI admin_UI;
    [SerializeField]
    private int ThisToggleNumber;

    //スクリプト定数
    const float HANDLE_MOVE_INTERVAL_TIME = 0.02f;
    const float HANDLE_MOVE_RANGE = 5.4f;
    const int MOBILE_MOVE_TIMES = 18;
    const int WINDOWS_MOVE_TIMES = 14;

    /// <summary>
    /// トグルの値
    /// </summary>
    public bool Value;
    
    private static readonly Color OFF_BG_COLOR = new Color(0.92f, 0.92f, 0.92f);
    private static readonly Color ON_BG_COLOR = new Color(0.2f, 0.84f, 0.3f);

    
    private void Start()
    {
        admin_UI = GetComponentInParent<Admin_UI>();
    }

    /// <summary>
    /// トグルのボタンアクションに設定しておく
    /// </summary>
    public void SwitchToggle()
    {
        Value = !Value;
        
        admin_UI.ChangeToggle(ThisToggleNumber);
        StartCoroutine("MoveHandle");
    }

    IEnumerator  MoveHandle()
    {
        var n = Value? HANDLE_MOVE_RANGE:-HANDLE_MOVE_RANGE;
        var s = admin_UI.isWorkeingMobilePlatform?MOBILE_MOVE_TIMES:WINDOWS_MOVE_TIMES;

        for (int i = 0; i < s; i++)
        {
            handle.position = handle.position + new Vector3(n ,0,0);
            yield return new WaitForSecondsRealtime(HANDLE_MOVE_INTERVAL_TIME);
        }
        backgroundImage.color = Value? ON_BG_COLOR : OFF_BG_COLOR;
    }

    public void ToggleOn()
    {
        Value = true;
        handle.localPosition = new Vector3(55,0,0);
        backgroundImage.color = ON_BG_COLOR;
    }
}