using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Analytics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum InventoryUIState { ItemSelection, PartySelection, Busy }

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemDescription;

    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;

    [Header("Party Screen")]
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] GameObject selectObject;
    [SerializeField] TextMeshProUGUI selectText;

    Inventory inventory;
    int selectedItem = 0;

    InventoryUIState state;

    List<ItemSlotUI> slotUIList;
    RectTransform itemListRect;

    public bool justOpenedBag;
    private bool justOpenedPartyScreen;

    const int itemsInViewport = 8;

    private void Awake()
    {
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdateItemList();
    }

    void UpdateItemList()
    {
        // Clear all the existing items
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }

        slotUIList = new List<ItemSlotUI>();
        foreach (var itemSlot in inventory.Slots)
        {
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetData(itemSlot);

            slotUIList.Add(slotUIObj);
        }

        UpdateItemSelection();
    }

    public void HandleBagUpdate(Action onBack)
    {
        if (justOpenedBag)
        {
            justOpenedBag = false;
            state = InventoryUIState.ItemSelection;
            return;
        }

        if (state == InventoryUIState.ItemSelection)
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

            selectedItem = Mathf.Clamp(selectedItem, 0, inventory.Slots.Count - 1);

            if (prevSelection != selectedItem)
            {
                UpdateItemSelection();
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                OpenPartyScreen();
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                onBack?.Invoke();
            }
        }
        else if (state == InventoryUIState.PartySelection)
        {
            // Overlap input errors fix:
            if (justOpenedPartyScreen)
            {
                justOpenedPartyScreen = false;
                return;
            }

            // Handle Party Selection
            Action onSelected = () =>
            {
                // Use item on selected Pokemon
            };

            Action onBackPartyScreen = () =>
            {
                ClosePartyScreen();
            };

            partyScreen.HandleUpdate(onSelected, onBackPartyScreen);
        }
    }

    void ClosePartyScreen()
    {
        state = InventoryUIState.ItemSelection;
        justOpenedBag = true;
        partyScreen.gameObject.SetActive(false);
        selectObject.SetActive(false);
    }

    void OpenPartyScreen()
    {
        state = InventoryUIState.PartySelection;
        partyScreen.Init();
        partyScreen.gameObject.SetActive(true);
        selectText.text = $"Select a Pokemon to use {inventory.Slots[selectedItem].Item.Name} on!";
        selectObject.SetActive(true);
        justOpenedPartyScreen = true;
    }

    void UpdateItemSelection()
    {
        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedItem)
            {
                slotUIList[i].NameText.color = Color.blue;
            }
            else slotUIList[i].NameText.color = Color.black;
        }

        var slot = inventory.Slots[selectedItem].Item;
        itemIcon.sprite = slot.Icon;
        itemDescription.text = slot.Description;

        HandleScrolling();
    }

    void HandleScrolling()
    {
        float scrollPos = Mathf.Clamp(selectedItem - itemsInViewport / 2, 0, selectedItem) * slotUIList[0].Height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);

        // Whether to show arrows
        bool showUpArrow = selectedItem > (itemsInViewport / 2);
        upArrow.gameObject.SetActive(showUpArrow);

        bool showDownArrow = selectedItem + itemsInViewport / 2 < slotUIList.Count;
        downArrow.gameObject.SetActive(showDownArrow);
    }
}
