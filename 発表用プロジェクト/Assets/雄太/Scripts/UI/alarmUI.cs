using UnityEngine;
using System.Collections;

public class alarmUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform rectTransform;//自身のUIトランスフォームを取得
    [SerializeField]
    private float destroyTime = 3;
private void Start() {
    rectTransform.localPosition = new Vector3(0,0,0);//ローカルポジションを中央にする
    StartCoroutine("Destor");
}

    IEnumerator Destor()
    {
        yield return new WaitForSecondsRealtime(destroyTime);
        Destroy(rectTransform.gameObject);
    }
}
