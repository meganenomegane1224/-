using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private ItemsDialog itemsDialog;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private InventoryManager inventory;
    [SerializeField] private GameObject Inventory;
    // [SerializeField] private ItemTransform itemPanelTransform;
    [SerializeField] private PPanelTransform pPanelTransform;
    [SerializeField] private GameObject inventoryUI;
    // [SerializeField] private PPanelTransform pPanelTransform;
    // [SerializeField] private MenuController menucontroller;

    private void Start()
    {
        Inventory.SetActive(false);
         // ポーズのパネルは初期状態では非表示にしておく
    }

    /// <summary>
    /// ゲームを一時停止します。
    /// </summary>
    private void Pause()
    {
        Time.timeScale = 0; // Time.timeScaleで時間の流れの速さを決める。0だと時間が停止する
        pausePanel.SetActive(true);
    }

    /// <summary>
    /// ゲームを再開します。
    /// </summary>
    private void Resume()
    {
        Time.timeScale = 1; // また時間が流れるようにする
        pausePanel.SetActive(false);
    }

    /// <summary>
    /// アイテムウインドウを開閉します。
    /// </summary>
    private void ToggleInventory()
    {
        inventory.Toggle1();
    }

    private void set(){
        itemsDialog.reflesh();
    }

    // private void transPanel(){
    //     itemPanelTransform.transItemPanelPos();
    //     // pPanelTransform.transPausePanelPos();
    // }

    /// <summary>
    /// レシピウインドウを開閉します。
    /// </summary>
    private void ToggleRecipeDialog()
    {
        // TODO 後で実装
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            GetKeyEscape(0);
        // menucontroller.ResetMenu();
        }
        if(Input.GetKeyDown(KeyCode.Q))
        {
            GetKeyEscape(0);
        }

        if(inventoryUI.activeSelf == false || true)
        {
            if(Input.GetKeyDown(KeyCode.B))
            {
                GetKeyEscape(1);
            
            }
            if(Input.GetKeyDown(KeyCode.V))
            {
                GetKeyEscape(2);
            
            }
            if(Input.GetKeyDown(KeyCode.T))
            {
                GetKeyEscape(3);
            }
        }
    // transPanel();

       if(Input.GetKeyDown(KeyCode.LeftAlt))
        {
            ShowCursor();
        }
        if(Input.GetKeyUp(KeyCode.LeftAlt) && inventoryUI.activeSelf == false)
        {
            HideCursor();
        }
    }

    public void GetKeyEscape(int n)
    {
        ToggleInventory();
    
        pPanelTransform.ResetMenu();


        if(n != 0 && inventoryUI.activeSelf == false)
        {
            ToggleInventory();
        }

        if(n == 1)
        {
            pPanelTransform.ChangeBagPanel();
            set();
        }
        else if(n == 2)
        {
            pPanelTransform.ChangeWeaponPanel();
        }
        else if (n == 3)
        {
            pPanelTransform.ChangeSkillTreePanel();
        }
    

    }
    public void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


}