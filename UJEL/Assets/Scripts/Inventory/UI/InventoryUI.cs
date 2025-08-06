using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Services.Analytics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum InventoryUIState { ItemSelection, PartySelection, MoveToForget, Busy }

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;

    [SerializeField] TextMeshProUGUI categoryText;
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemDescription;

    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;

    [Header("Party Screen")]
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] GameObject selectObject;
    [SerializeField] TextMeshProUGUI selectText;

    [Header("Move Selection")]
    [SerializeField] MoveSelectionUI moveSelectionUI;

    Inventory inventory;
    int selectedItem = 0;
    int selectedCategory = 0;
    MoveBase moveToLearn;

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

        inventory.OnUpdated += UpdateItemList;
    }

    void UpdateItemList()
    {
        // Clear all the existing items
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }

        slotUIList = new List<ItemSlotUI>();
        foreach (var itemSlot in inventory.GetSlotsByCategory(selectedCategory))
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
            int prevCategory = selectedCategory;

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                ++selectedItem;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                --selectedItem;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ++selectedCategory;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                --selectedCategory;
            }

            // Wrap selectedCategory
            if (selectedCategory > Inventory.ItemCategories.Count - 1)
            {
                selectedCategory = 0;
            }
            else if (selectedCategory < 0)
            {
                selectedCategory = Inventory.ItemCategories.Count - 1;
            }

            selectedItem = Mathf.Clamp(selectedItem, 0, inventory.GetSlotsByCategory(selectedCategory).Count - 1);


            if (prevCategory != selectedCategory)
            {
                ResetSelection();
                categoryText.text = Inventory.ItemCategories[selectedCategory];
                UpdateItemList();
            }
            else if (prevSelection != selectedItem)
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
                StartCoroutine(UseItem());
            };

            Action onBackPartyScreen = () =>
            {
                ClosePartyScreen();
            };

            partyScreen.HandleUpdate(onSelected, onBackPartyScreen);
        }
        else if (state == InventoryUIState.MoveToForget)
        {
            Action<int> onMoveSelected = (int moveIndex) =>
            {
                StartCoroutine(OnMoveToForgetSelected(moveIndex));
            };

            moveSelectionUI.HandleMoveSelection(onMoveSelected);
        }
    }

    void ResetSelection()
    {
        selectedItem = 0;

        upArrow.gameObject.SetActive(false);
        downArrow.gameObject.SetActive(false);

        itemIcon.sprite = null;
        itemDescription.text = "";
    }

    IEnumerator UseItem()
    {
        state = InventoryUIState.Busy;

        yield return StartCoroutine(HandleTmItems());

        var usedItem = inventory.UseItem(selectedItem, partyScreen.SelectedMember, selectedCategory);
        if (usedItem != null)
        {
            if (usedItem is RecoveryItem)
                yield return DialogManager.Instance.ShowDialogText($"The player used {usedItem.Name}.");
            else if (usedItem is LureItem)
                yield return DialogManager.Instance.ShowDialogText($"The player used {usedItem.Name}.");
        }
        else
        {
            if (usedItem is LureItem)
                yield return DialogManager.Instance.ShowDialogText($"A lure is already active.");
            else
            {
                yield return DialogManager.Instance.ShowDialogText($"You can't use that item here.");
            }
        }

        ClosePartyScreen();
    }

    IEnumerator HandleTmItems()
    {
        var tmItem = inventory.GetItem(selectedItem, selectedCategory) as TmItem;

        if (tmItem == null) yield break;

        var pokemon = partyScreen.SelectedMember;

        if (pokemon.HasMove(tmItem.Move))
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} already knows {tmItem.Move.Name}.");
            yield break;
        }

        if (!tmItem.CanBeTaught(pokemon))
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} can't learn {tmItem.Move.Name}.");
            yield break;
        }

        if (pokemon.Moves.Count < PokemonBase.MaxNumOfMoves)
        {
            pokemon.LearnMove(tmItem.Move);
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} learned {tmItem.Move.Name}!");
        }
        else
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} is trying to learn {tmItem.Move.Name}.");
            yield return DialogManager.Instance.ShowDialogText($"But it cannot learn more than {PokemonBase.MaxNumOfMoves} moves.");
            yield return ChooseMoveToForget(pokemon, tmItem.Move);
            yield return new WaitUntil(() => state != InventoryUIState.MoveToForget);
        }
    }

    IEnumerator ChooseMoveToForget(Pokemon pokemon, MoveBase newMove)
    {
        state = InventoryUIState.Busy;
        yield return DialogManager.Instance.ShowDialogText($"Forget a move?");
        yield return new WaitForSeconds(0.75f);
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(pokemon.Moves.Select(x => x.Base).ToList(), newMove);
        moveToLearn = newMove;

        state = InventoryUIState.MoveToForget;
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
        selectText.text = $"Select a Pokemon to use {inventory.GetSlotsByCategory(selectedCategory)[selectedItem].Item.Name} on!";
        selectObject.SetActive(true);
        justOpenedPartyScreen = true;
    }

    void UpdateItemSelection()
    {
        var slots = inventory.GetSlotsByCategory(selectedCategory);

        selectedItem = Mathf.Clamp(selectedItem, 0, slots.Count - 1);

        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedItem)
            {
                slotUIList[i].NameText.color = Color.blue;
            }
            else slotUIList[i].NameText.color = Color.black;
        }

        if (slots.Count > 0)
        {
            var slot = slots[selectedItem].Item;
            itemIcon.sprite = slot.Icon;
            itemDescription.text = slot.Description;
            HandleScrolling();
        }
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

    IEnumerator OnMoveToForgetSelected(int moveIndex)
    {
        var pokemon = partyScreen.SelectedMember;

        moveSelectionUI.gameObject.SetActive(false);
        if (moveIndex == PokemonBase.MaxNumOfMoves)
        {
            // Don't learn the new move
            yield return StartCoroutine(DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} did not learn {moveToLearn.Name}."));
        }
        else
        {
            // Forget selected move and learn the new move
            var selectedMove = pokemon.Moves[moveIndex].Base;
            pokemon.Moves[moveIndex] = new Move(moveToLearn);
            yield return StartCoroutine(DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} forgot {selectedMove.Name} and learned {moveToLearn.Name}!"));
        }

        moveToLearn = null;
        state = InventoryUIState.ItemSelection;
    }
}
