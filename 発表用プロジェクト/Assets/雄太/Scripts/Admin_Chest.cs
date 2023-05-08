using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Admin_Chest : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem particleSystem; //アタッチ
    [SerializeField]
    private Item[] item;                   //放出するアイテムを配列化して保存　これはprefabuとして作成し、それを入れる

    const float OUT_ITEM_WAIT_TIME = 0.5f;
    const float ITME_MOVE_RANGE_X = 0.5f;
    const float ITME_MOVE_RANGE_Y = 0.3f;
    public void Open()
    {
        GetComponent<Animator>().SetTrigger("Open");//宝箱を開けるアニメーションを再生する
        GetComponent<AudioSource>().Play();//開封音を再生
        particleSystem.Play();//開封エフェクトを再生
        StartCoroutine("OutItem");//アイテムを放出するコルーチンを起動する
    }

    IEnumerator OutItem()
    {
        yield return new WaitForSeconds(OUT_ITEM_WAIT_TIME);//放出可能時間まで待つ
        foreach (var item in item)//すべてのアイテムをインスタンス化し、放出　dotweenを利用してランダム位置に移動させる
        {
            var o = Instantiate(item , transform.position , transform.rotation , this.transform);
            o.transform.DOLocalMove(new Vector3(Random.Range(-ITME_MOVE_RANGE_X , ITME_MOVE_RANGE_X) , 0 , Random.Range(ITME_MOVE_RANGE_Y , 1)) , 0.2f).SetEase(Ease.OutCubic);
        }
    }
}
