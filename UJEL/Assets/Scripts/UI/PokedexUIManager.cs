using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum PokedexUIState { Pokedex, Busy }

public class PokedexUIManager : MonoBehaviour
{
    [SerializeField] private GameObject pokedexElementPrefab;
    [SerializeField] private Transform contentParent;
    [SerializeField] private Image largePokemonImage;
    [SerializeField] private TextMeshProUGUI largePokemonNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject upArrow;
    [SerializeField] private GameObject downArrow;
    [SerializeField] private RectTransform itemListRect;

    public static PokedexUIManager instance;

    public bool justOpenedPokedex = false;

    private List<PokedexEntry> pokedex; // Pokedex directly from PokedexManager.
    private List<PokedexElementUI> pokedexUIList; // Links a pokedexEntry to its UI.

    private PokedexUIState state; // Stores the state of the menu.
    private int selectedItem;
    const int itemsInViewport = 8;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }   
    }

    public void Start()
    {
        pokedex = PokedexManager.instance.Pokedex;

        RefreshPokedex();
    }

    public void RefreshPokedex()
    {
        // Clear anything previously in the UI
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        pokedexUIList = new List<PokedexElementUI>();

        // Make each element and set its data
        foreach (var entry in pokedex)
        {
            var obj = Instantiate(pokedexElementPrefab, contentParent);
            var currElementUI = obj.GetComponent<PokedexElementUI>();
            currElementUI.SetData(entry);

            // Add the object to the PokedexElementUIList to link an entry to its UI
            pokedexUIList.Add(currElementUI);
        }

        UpdateItemSelection();
    }

    public void SetCaughtStatus(PokemonBase pokemon, bool caught)
    {
        int index = pokedex.FindIndex(e => e.pokemon == pokemon);
        if (index != -1)
        {
            pokedex[index].SetCaught(caught);

            if (index < pokedexUIList.Count)
            {
                pokedexUIList[index].SetData(pokedex[index]);
            }
        }
    }

    public void HandleUpdate(Action onBack)
    {
        if (justOpenedPokedex)
        {
            justOpenedPokedex = false;
            state = PokedexUIState.Pokedex;
            return;
        }

        if (state == PokedexUIState.Pokedex)
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

            selectedItem = Mathf.Clamp(selectedItem, 0, pokedex.Count - 1);

            if (prevSelection != selectedItem)
            {
                UpdateItemSelection();
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                onBack?.Invoke();
            }
        }
    }

    void UpdateItemSelection()
    {
        for (int i = 0; i < pokedexUIList.Count; i++)
        {
            if (i == selectedItem)
            {
                pokedexUIList[i].SetHighlighted(true);
            }
            else pokedexUIList[i].SetHighlighted(false);
        }

        // Change the large items to the current hovered item
        largePokemonImage.sprite = pokedexUIList[selectedItem].PokemonImage.sprite;
        if (pokedex[selectedItem].haveCaught) largePokemonImage.color = Color.white;
        else largePokemonImage.color = Color.black;

        largePokemonNameText.text = $"{pokedexUIList[selectedItem].NameText.text}";
        descriptionText.text = $"{pokedexUIList[selectedItem].DescriptionText}";

        HandleScrolling();
    }

    void HandleScrolling()
    {
        float scrollPos = Mathf.Clamp(selectedItem - itemsInViewport / 2, 0, selectedItem) * pokedexUIList[0].Height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);

        // Whether to show arrows
        bool showUpArrow = selectedItem > (itemsInViewport / 2);
        upArrow.gameObject.SetActive(showUpArrow);

        bool showDownArrow = selectedItem + itemsInViewport / 2 < pokedexUIList.Count;
        downArrow.gameObject.SetActive(showDownArrow);
    }
}
