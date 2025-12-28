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

    [Header("Text Objects")]
    [SerializeField] TextMeshProUGUI moneyTxt;
    [SerializeField] TextMeshProUGUI inBagTxt;

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

        UpdateMoneyText();
        UpdateInBagText();
        UpdateItemSelection();
    }

    private void UpdateMoneyText()
    {
        string currMoney = MoneyHandler.instance.Money.ToString();
        moneyTxt.text = $"Money: {currMoney}";
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
            Purchase(1);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Purchase(10);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            ExitShop();
        }
    }

    public void Purchase(int numOfItems = 1)
    {
        var slot = shopSlotUIs[selectedItem].shopSlot;

        // Return if the amount bought would lower the item count below 0
        if ((slot.itemCount - numOfItems) < 0 && slot.infiniteNumOfItem != true)
        {
            Debug.Log("There are no items left!");
            return;
        }

        var item = slot.item;
        int cost = slot.cost * numOfItems;
        var playerInventory = Inventory.GetInventory();

        if (MoneyHandler.instance.CanBuy(cost))
        {
            MoneyHandler.instance.RemoveMoney(cost);

            playerInventory.AddItem(item, numOfItems);

            if (!slot.infiniteNumOfItem)
            {
                slot.itemCount -= numOfItems;
                shopSlotUIs[selectedItem].UpdateCountText(slot);
            }

            UpdateInBagText();
            UpdateMoneyText();
        }
        else
        {
            Debug.Log("You don't have enough money to buy the item.");
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

        UpdateInBagText();
    }

    private void UpdateInBagText()
    {
        if (shopSlotUIs.Count == 0) return;

        var slot = shopSlotUIs[selectedItem].shopSlot;
        var item = slot.item;

        int countInBag = Inventory.GetInventory().GetItemCount(item);
        inBagTxt.text = $"In Bag: {countInBag}";
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
