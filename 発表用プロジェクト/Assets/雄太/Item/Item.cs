using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType
    {
        Wood,
        Stone,
        ThrowAxe,
        Tree
    }

    [SerializeField] private ItemType type;

    public void Initialize()
    {
        var transformCache = transform;
        var dropPosition = transform.localPosition +
            new Vector3(Random.Range(-1f,1f),0,Random.Range(-1f,1f));
        var defaultScale = transformCache.localScale;
        transformCache.localScale = Vector3.zero;
    }
    public void GetItem()
    {

        OwnedItemsData.Instance.Add(type);
        OwnedItemsData.Instance.Save();
        foreach (var item in OwnedItemsData.Instance.OwnedItems)
        {
        Debug.Log(item.Type + "を" + item.Number + "個所持");
        }
        Destroy(gameObject);
    }
}