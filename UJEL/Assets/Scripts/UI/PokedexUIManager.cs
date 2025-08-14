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
    [SerializeField] private TextMeshProUGUI locationText;
    [SerializeField] private TextMeshProUGUI type1text;
    [SerializeField] private TextMeshProUGUI type2text;
    [SerializeField] private TextMeshProUGUI heightTxt;
    [SerializeField] private TextMeshProUGUI weightTxt;
    [SerializeField] private TextMeshProUGUI evolutionsTxt;
    [SerializeField] private GameObject upArrow;
    [SerializeField] private GameObject downArrow;
    [SerializeField] private RectTransform itemListRect;

    public static PokedexUIManager instance;

    public bool justOpenedPokedex = false;

    private List<PokedexEntry> pokedex; // Pokedex directly from PokedexManager.
    private List<PokedexElementUI> pokedexUIList; // Links a pokedexEntry to its UI.

    public List<PokedexElementUI> PokedexUIList => pokedexUIList;

    private PokedexUIState state; // Stores the state of the menu.
    private int selectedItem;

    // Holding keys
    float inputRepeatDelay = 0.1f; // 10 times per second
    private float initialDelay = 0.7f;
    private bool isHoldingKey = false;
    private float keyHoldTimer = 0f;
    private int holdDirection = 0; // -1 for up, 1 for down


    // Items for setting the scroll
    const int itemsInViewport = 6;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }   
    }

    public void Start()
    {
        RefreshPokedex();
    }

    public void RefreshPokedex()
    {
        pokedex = PokedexManager.instance.Pokedex;

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

    public void HandleUpdate(Action onBack)
    {
        // Fix input bugs
        if (justOpenedPokedex)
        {
            justOpenedPokedex = false;
            state = PokedexUIState.Pokedex;
            return;
        }

        if (state == PokedexUIState.Pokedex)
        {
            int prevSelection = selectedItem;

        // Read directional key states
        bool keyDown = Input.GetKey(KeyCode.DownArrow);
        bool keyUp = Input.GetKey(KeyCode.UpArrow);
        bool keyLeft = Input.GetKey(KeyCode.LeftArrow);
        bool keyRight = Input.GetKey(KeyCode.RightArrow);

        int direction = 0;
        int skipAmount = 1; // The number of inputs to jump

            // --- Up/Down Input (Priority) ---
            if (Input.GetKeyDown(KeyCode.DownArrow)) // Pressing down
            {
                direction = 1;
                skipAmount = 1;
                isHoldingKey = true;
                keyHoldTimer = Time.time + initialDelay;
                holdDirection = direction;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow)) // Pressing up
            {
                direction = -1;
                skipAmount = 1;
                isHoldingKey = true;
                keyHoldTimer = Time.time + initialDelay;
                holdDirection = direction;
            }
            else if (isHoldingKey && (keyDown || keyUp)) // Holding down or up
            {
                if (Time.time >= keyHoldTimer)
                {
                    direction = holdDirection;
                    skipAmount = 1;
                    keyHoldTimer = Time.time + inputRepeatDelay;
                }
            }
            else if (!keyDown && !keyUp) // Not pressing up or down
            {
                isHoldingKey = false;

                // --- Left/Right Input (only if Up/Down not held) ---
                if (Input.GetKeyDown(KeyCode.RightArrow)) // Press right
                {
                    direction = 1;
                    skipAmount = 10;
                    isHoldingKey = true;
                    keyHoldTimer = Time.time + initialDelay;
                    holdDirection = direction;
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow)) // Press left
                {
                    direction = -1;
                    skipAmount = 10;
                    isHoldingKey = true;
                    keyHoldTimer = Time.time + initialDelay;
                    holdDirection = direction;
                }
                else if ((keyLeft || keyRight) && isHoldingKey) // Hold left or right
                {
                    if (Time.time >= keyHoldTimer)
                    {
                        direction = holdDirection;
                        skipAmount = 10;
                        keyHoldTimer = Time.time + inputRepeatDelay;
                    }
                }
                else if (!keyLeft && !keyRight) // Not pressing any keys
                {
                    isHoldingKey = false;
                }
            }

            // Apply direction change
            if (direction != 0)
            {
                selectedItem += direction * skipAmount;
                selectedItem = Mathf.Clamp(selectedItem, 0, pokedex.Count - 1);

                if (selectedItem != prevSelection)
                {
                    UpdateItemSelection();
                }
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
        locationText.text = $"Locations: {pokedexUIList[selectedItem].LocationText}";
        heightTxt.text = $"Height: {pokedexUIList[selectedItem].Heightm} m.";
        weightTxt.text = $"Weight: {pokedexUIList[selectedItem].Weightkg} kg.";
        type1text.text = pokedexUIList[selectedItem].Type1;
        type2text.text = pokedexUIList[selectedItem].Type2;
        evolutionsTxt.text = pokedexUIList[selectedItem].EvolutionsText;

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
