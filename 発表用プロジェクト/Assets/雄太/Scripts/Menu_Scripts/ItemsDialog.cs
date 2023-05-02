using UnityEngine;

public class ItemsDialog : MonoBehaviour
{
    [SerializeField] private int buttonNumber = 15;
    [SerializeField] private ItemButton itemButton;

    private ItemButton[] _itemButtons;

    private void Start()
    {
        // アイテム欄を必要な分だけ複製
        for (var i = 0; i < buttonNumber - 1; i++)
        {
            Instantiate(itemButton, transform);
        }   

        // 子要素のItemButtonを一括取得、保持しておく
        _itemButtons = GetComponentsInChildren<ItemButton>();

          for (var i = 0; i < buttonNumber; i++)
            {
                // 各アイテムボタンに所持アイテム情報をセット
                _itemButtons[i].OwnedItem = OwnedItemsData.Instance.OwnedItems.Length > i 
                    ? OwnedItemsData.Instance.OwnedItems[i]
                    : null;
            }
    }

    /// <summary>
    /// アイテム欄の表示/非表示を切り替えます。
    /// </summary>
    public void reflesh()
    {
        if (gameObject.activeSelf)
        {
            // 表示された場合はアイテム欄をリフレッシュする
            for (var i = 0; i < buttonNumber; i++)
            {
                // 各アイテムボタンに所持アイテム情報をセット
                _itemButtons[i].OwnedItem = OwnedItemsData.Instance.OwnedItems.Length > i 
                    ? OwnedItemsData.Instance.OwnedItems[i]
                    : null;
            }

        }
    }
}