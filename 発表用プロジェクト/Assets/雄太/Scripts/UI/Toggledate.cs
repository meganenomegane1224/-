using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Toggledate : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;//背景画像をアタッチ
    [SerializeField] private RectTransform handle;//ハンドル部分の画像をアタッチ
    private Admin_UI admin_UI;//アタッチ
    [SerializeField]
    private int ThisToggleNumber;//このトグルの個別番号を定義　トグルによって命令を出すときに使われる

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
        admin_UI = GetComponentInParent<Admin_UI>();//取得
    }

    /// <summary>
    /// トグルのボタンアクションに設定しておく
    /// </summary>
    public void SwitchToggle()
    {
        Value = !Value;//オンオフを変更
        
        admin_UI.ChangeToggle(ThisToggleNumber);//個別トグルナンバーに応じた引数で感知メソッドを実行
        StartCoroutine("MoveHandle");//ハンドルを動かすコルーチンを起動
    }

    IEnumerator  MoveHandle()
    {
        var n = Value? HANDLE_MOVE_RANGE:-HANDLE_MOVE_RANGE;//オンオフに応じて、ハンドルの色を変更
        var s = admin_UI.isWorkeingMobilePlatform?MOBILE_MOVE_TIMES:WINDOWS_MOVE_TIMES;//モバイルで動いているかいないかでハンドルの移動量を変更

        for (int i = 0; i < s; i++)//規定回数ハンドルを移動させる　　　　　　dotweenに書き換え予定
        {
            handle.position = handle.position + new Vector3(n ,0,0);
            yield return new WaitForSecondsRealtime(HANDLE_MOVE_INTERVAL_TIME);
        }
        backgroundImage.color = Value? ON_BG_COLOR : OFF_BG_COLOR;
    }

    //ゲーム開始時、保存　されていたトグルがオンなら実行される関数
    public void ToggleOn()
    {
        Value = true;//オンにする
        handle.localPosition = new Vector3(55,0,0);//位置を固定
        backgroundImage.color = ON_BG_COLOR;//カラーを変える
    }
}