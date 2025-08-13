using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    public static ShopController instance;

    public List<ShopSlot> currShopSlots;
    public List<ShopSlotUI> shopSlotUIs;

    [Header("UI Objects")]
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemDescription;

    [Header("Slots Details")]
    [SerializeField] GameObject ShopUI;
    [SerializeField] Transform slotParent;
    [SerializeField] ShopSlotUI slotPrefab;

    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;

    private int selectedItem;
    const int itemsInViewport = 8;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public IEnumerator OpenShop(List<ShopSlot> shopSlots)
    {
        if (shopSlots == null)
        {
            Debug.LogError("No shop slots for the shop.");
            yield break;
        }

        currShopSlots = shopSlots;

        GameController.instance.state = GameState.Shop;

        GameController.instance.SetUICanvas(true);
        ShopUI.SetActive(true);
        selectedItem = 0;

        Init();

        yield return null;
    }

    public void Init()
    {
        shopSlotUIs = new List<ShopSlotUI>();

        // Clear old UI
        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }

        // Create new UI
        foreach (ShopSlot slot in currShopSlots)
        {
            ShopSlotUI ui = Instantiate(slotPrefab, slotParent);
            shopSlotUIs.Add(ui);
            ui.Init(slot);
        }

        UpdateItemSelection();
    }

    public void HandleUpdate()
    {
        int prevSelection = selectedItem;

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ++selectedItem;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            --selectedItem;
        }

        selectedItem = Mathf.Clamp(selectedItem, 0, shopSlotUIs.Count - 1);

        if (prevSelection != selectedItem)
        {
            UpdateItemSelection();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var slot = shopSlotUIs[selectedItem].shopSlot;

            // Return if it is out of the item count and not infinite
            if (slot.itemCount <= 0 && slot.infiniteNumOfItem != true)
            {
                Debug.Log("There are no items left!");
                return;
            }

            var item = slot.item;
            int cost = slot.cost;
            var playerInventory = Inventory.GetInventory();

            if (MoneyHandler.instance.CanBuy(cost))
            {
                MoneyHandler.instance.RemoveMoney(cost);
                playerInventory.AddItem(item);

                if (slot.infiniteNumOfItem)
                {
                    return;
                }
                else
                {
                    slot.itemCount -= 1;
                    shopSlotUIs[selectedItem].UpdateCountText(slot);
                }
            }
            else
            {
                Debug.Log("You don't have enough money to buy the item.");
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            ExitShop();
        }
    }

    public void ExitShop()
    {
        Debug.Log("Leaving shop.");
        GameController.instance.state = GameState.FreeRoam;

        GameController.instance.SetUICanvas(false);
        ShopUI.SetActive(false);
    }
    
    void UpdateItemSelection()
    {
        //Debug.Log("Updating Item Selection");

        selectedItem = Mathf.Clamp(selectedItem, 0, shopSlotUIs.Count - 1);

        for (int i = 0; i < shopSlotUIs.Count; i++)
        {
            if (i == selectedItem)
            {
                shopSlotUIs[i].Highlight(true);
            }
            else shopSlotUIs[i].Highlight(false);
        }

        if (shopSlotUIs.Count > 0)
        {
            var slot = shopSlotUIs[selectedItem].shopSlot.item;
            itemIcon.sprite = slot.Icon;
            itemDescription.text = slot.Description;
            HandleScrolling();
        }
    }

    void HandleScrolling()
    {
        float scrollPos = Mathf.Clamp(selectedItem - itemsInViewport / 2, 0, selectedItem) * shopSlotUIs[0].Height;
        slotParent.localPosition = new Vector2(slotParent.localPosition.x, scrollPos);

        // Whether to show arrows
        bool showUpArrow = selectedItem > (itemsInViewport / 2);
        upArrow.gameObject.SetActive(showUpArrow);

        bool showDownArrow = selectedItem + itemsInViewport / 2 < shopSlotUIs.Count;
        downArrow.gameObject.SetActive(showDownArrow);
    }
}
