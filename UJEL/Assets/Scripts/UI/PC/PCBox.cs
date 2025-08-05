using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public enum SortMode
{
    AZ,
    ZA,
    DexNumber,
    Type,
    Level,
    LivingDex,
    Custom
}

public class PCBox : MonoBehaviour
{
    [Header("PC")]
    [SerializeField] public GameObject pcObject;
    public List<Pokemon> storedPokemon = new List<Pokemon>(); // This is the PC Box

    public static PCBox instance;

    [Header("Slots")]
    [SerializeField] private List<BoxStorageSlotUI> boxStorageSlots;
    [SerializeField] private List<BoxPartySlotUI> boxPartySlots;

    private Vector2Int gridCursorPos = new Vector2Int(0, 0);

    [Header("Sorting")]
    [SerializeField] private SortMode currentSort = SortMode.Custom;
    [SerializeField] TextMeshProUGUI SortText;

    [Header("Boxes")]
    [SerializeField] TextMeshProUGUI boxName;
    [SerializeField] private List<BoxData> boxes = new List<BoxData>();

    private const int BOX_SIZE = 35;
    private const int MAX_BOXES = 20;
    private int currentBoxIndex = 0;
    public BoxData CurrentBox => boxes[currentBoxIndex];
    private List<string> boxNames = new List<string>
    {
        "Box 1", "Box 2", "Box 3", "Box 4", "Box 5", "Box 6", "Box 7",
        "Box 8", "Box 9", "Box 10", "Box 11", "Box 12", "Box 13",
        "Box 14", "Box 15", "Box 16", "Box 17", "Box 18", "Box 19", "Box 20"
    };

    [Header("Hovered Pokemon")]
    [SerializeField] HoveredPokemonUI hoveredPokemonUI;

    // Layers
    private int currentLayer = 1;
    // 0   -> Box Navigation
    // 1-5 -> Grid
    // 6   -> Sorting Layer
    // 7   -> Party Layer

    const int BOX_NAVIGATION_LAYER = 0;
    const int GRID_LAYER_MIN = 1;
    const int GRID_LAYER_MAX = 5;
    const int SORTING_LAYER = 6;
    const int PARTY_LAYER = 7;

    public int boxSize => BOX_SIZE;

    private float delaySwitchBox = 0.75f;
    private float lastSwitchTime = -Mathf.Infinity;
    private int partyCursor = 0;

    const int GRID_COLS = 7;
    const int GRID_ROWS = 5;
    const int MAX_PARTY_SIZE = 6;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void InitializeBoxes()
    {
        boxes.Clear();
        for (int i = 0; i < MAX_BOXES; i++)
        {
            boxes.Add(new BoxData($"Box: {i + 1}"));
        }
    }

    public void HandlePCUpdate()
    {
        if (Input.GetKeyDown(KeyCode.A)) SortPC(currentSort);
        else if (Input.GetKeyDown(KeyCode.X)) ClosePCBox();

        if (currentLayer >= GRID_LAYER_MIN && currentLayer <= GRID_LAYER_MAX)
        {
            HandleGridNavigation();
        }
        else if (currentLayer == BOX_NAVIGATION_LAYER)
        {
            HandleBoxNavigation();
        }
        else if (currentLayer == SORTING_LAYER)
        {
            HandleSortMode();
        }
        else if (currentLayer == PARTY_LAYER)
        {
            HandlePartyNavigation();
        }
    }

    private void HandlePartyNavigation()
    {
        int prevIndex = partyCursor;

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (partyCursor < MAX_PARTY_SIZE - 1)
            {
                partyCursor++;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (partyCursor > 0)
            {
                partyCursor--;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // Go back to grid
            boxPartySlots[prevIndex].ImageSlot.OnSelectionChanged(false);
            gridCursorPos = new Vector2Int(0, 0);
            ChangeLayer(GRID_LAYER_MIN);
            return;
        }
        else return;

        boxPartySlots[prevIndex].ImageSlot.OnSelectionChanged(false);
        boxPartySlots[partyCursor].ImageSlot.OnSelectionChanged(true);
    }

    private void HandleBoxNavigation()
    {
        if (Time.time - lastSwitchTime < delaySwitchBox) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentBoxIndex = (currentBoxIndex - 1 + boxNames.Count) % boxNames.Count;
            UpdateBoxDisplay();
            lastSwitchTime = Time.time;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentBoxIndex = (currentBoxIndex + 1) % boxNames.Count;
            UpdateBoxDisplay();
            lastSwitchTime = Time.time;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangeLayer(GRID_LAYER_MIN);
        }
    }

    private void UpdateBoxDisplay()
    {
        boxName.text = boxNames[currentBoxIndex];
        RefreshCurrentBoxUI();
    }

    private void HandleGridNavigation()
    {
        int prevIndex = GetGridIndex(gridCursorPos);

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (gridCursorPos.x < GRID_COLS - 1)
                gridCursorPos.x += 1;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (gridCursorPos.x == 0)
            {
                boxStorageSlots[prevIndex].ImageSlot.OnSelectionChanged(false);
                ChangeLayer(PARTY_LAYER);
                return;
            }
            gridCursorPos.x -= 1;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (gridCursorPos.y == 0)
            {
                ChangeLayer(BOX_NAVIGATION_LAYER);
                boxStorageSlots[prevIndex].ImageSlot.OnSelectionChanged(false);
                return;
            }
            gridCursorPos.y = (gridCursorPos.y - 1 + GRID_ROWS) % GRID_ROWS;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (gridCursorPos.y == GRID_ROWS - 1)
            {
                boxStorageSlots[prevIndex].ImageSlot.OnSelectionChanged(false);
                ChangeLayer(SORTING_LAYER);
                return;
            }
            gridCursorPos.y = (gridCursorPos.y + 1) % GRID_ROWS;
        }
        else return;

        UpdateGridSelection(prevIndex);
    }

    private int GetGridIndex(Vector2Int pos)
    {
        return pos.y * GRID_COLS + pos.x;
    }

    private void UpdateGridSelection(int previousIndex)
    {
        int newIndex = GetGridIndex(gridCursorPos);

        boxStorageSlots[previousIndex].ImageSlot.OnSelectionChanged(false);
        boxStorageSlots[newIndex].ImageSlot.OnSelectionChanged(true);

        // Update the hoveredPokemonUI
        hoveredPokemonUI.Show(boxStorageSlots[newIndex].ImageSlot.PokemonSlotUI);
    }

    private void HandleSortMode()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            switch (currentSort)
            {
                case SortMode.AZ:
                    currentSort = SortMode.ZA;
                    break;
                case SortMode.ZA:
                    currentSort = SortMode.DexNumber;
                    break;
                case SortMode.DexNumber:
                    currentSort = SortMode.Level;
                    break;
                case SortMode.Level:
                    currentSort = SortMode.Type;
                    break;
                case SortMode.Type:
                    currentSort = SortMode.LivingDex;
                    break;
                case SortMode.LivingDex:
                    currentSort = SortMode.Custom;
                    break;
                case SortMode.Custom:
                    currentSort = SortMode.AZ;
                    break;
                default:
                    currentSort = SortMode.Custom;
                    break;
            }
            UpdateSortModeUI();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            switch (currentSort)
            {
                case SortMode.AZ:
                    currentSort = SortMode.Custom;
                    break;
                case SortMode.ZA:
                    currentSort = SortMode.AZ;
                    break;
                case SortMode.DexNumber:
                    currentSort = SortMode.ZA;
                    break;
                case SortMode.Level:
                    currentSort = SortMode.DexNumber;
                    break;
                case SortMode.Type:
                    currentSort = SortMode.Level;
                    break;
                case SortMode.LivingDex:
                    currentSort = SortMode.Type;
                    break;
                case SortMode.Custom:
                    currentSort = SortMode.LivingDex;
                    break;
                default:
                    currentSort = SortMode.Custom;
                    break;
            }
            UpdateSortModeUI();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangeLayer(GRID_LAYER_MAX);
        }
    }

    private void ChangeLayer(int newLayer)
    {
        ClearAllSlots();
        hoveredPokemonUI.Hide();

        currentLayer = newLayer;

        if (currentLayer == SORTING_LAYER) // Switching into Sorting
            SortText.color = Color.blue;
        else SortText.color = Color.white;

        if (currentLayer >= GRID_LAYER_MIN && currentLayer <= GRID_LAYER_MAX)
        {
            boxStorageSlots[GetGridIndex(gridCursorPos)].ImageSlot.OnSelectionChanged(true); // Highlight when you switch into the grid
            hoveredPokemonUI.Show(boxStorageSlots[GetGridIndex(gridCursorPos)].ImageSlot.PokemonSlotUI);
        }

        if (currentLayer == BOX_NAVIGATION_LAYER) // Switching into box
            boxName.color = Color.blue;
        else boxName.color = Color.white;

        if (currentLayer == PARTY_LAYER) // Switching into party
        {
            partyCursor = 0;
            boxPartySlots[partyCursor].ImageSlot.OnSelectionChanged(true);
        }
    }

    private void ClearAllSlots()
    {
        foreach (var boxStorageSlot in boxStorageSlots)
        {
            boxStorageSlot.ImageSlot.Clear();
        }
        foreach (var partySlot in boxPartySlots)
        {
            partySlot.ImageSlot.Clear();
        }
    }

    private void UpdateSortModeUI()
    {
        SortText.text = currentSort.ToString();
    }

    public void SortPC(SortMode mode)
    {
        switch (mode)
        {
            case SortMode.AZ:
                storedPokemon = storedPokemon.OrderBy(p => p.Base.Name).ToList();
                break;
            case SortMode.ZA:
                storedPokemon = storedPokemon.OrderByDescending(p => p.Base.Name).ToList();
                break;
            case SortMode.DexNumber:
                storedPokemon = storedPokemon.OrderBy(p => p.Base.PokedexNumber).ToList();
                break;
            case SortMode.Type:
                storedPokemon = storedPokemon.OrderBy(p => p.Base.type1).ThenBy(p => p.Base.type2).ToList();
                break;
            case SortMode.Level:
                storedPokemon = storedPokemon.OrderByDescending(p => p.Level).ToList();
                break;
            case SortMode.LivingDex:
                storedPokemon = GetLivingDexSortedList();
                break;
        }
    }

    private List<Pokemon> GetLivingDexSortedList()
    {
        var livingDexDict = new Dictionary<int, Pokemon>(); // DexNum -> Pokemon

        foreach (var p in storedPokemon)
        {
            int dexNum = p.Base.PokedexNumber; // Store the dex number

            // If it's the first pokemon with that Dex number, add it. If it is a duplicate but has a higher level, replace it.
            if (!livingDexDict.ContainsKey(dexNum) || p.Level > livingDexDict[dexNum].Level)
                livingDexDict[dexNum] = p;
        }

        var livingDexSorted = livingDexDict.Values.OrderBy(p => p.Base.PokedexNumber).ToList();
        var leftovers = storedPokemon.Except(livingDexSorted).ToList();

        return livingDexSorted.Concat(leftovers).ToList();
    }

    public void OpenPCBox()
    {
        // Turn on objects
        GameController.instance.SetUICanvas(true);
        pcObject.SetActive(true);
        GameController.instance.state = GameState.PC;

        // Reset colors and text
        ClearAllSlots();
        boxName.color = Color.white;
        SortText.color = Color.white;
        currentSort = SortMode.Custom;
        UpdateSortModeUI();

        // Initialize Pokemon Party Slots and Box Storage Slots
        currentBoxIndex = 0;
        InitializePokemonParty();
        SetPokemonInBoxes(storedPokemon);
        InitializeBoxSlots();

        // Reset selection and image
        currentLayer = 1;
        gridCursorPos = new Vector2Int(0, 0);
        boxStorageSlots[GetGridIndex(gridCursorPos)].ImageSlot.OnSelectionChanged(true);
        hoveredPokemonUI.Show(boxStorageSlots[GetGridIndex(gridCursorPos)].ImageSlot.PokemonSlotUI);
    }

    public void ClosePCBox()
    {
        boxStorageSlots[GetGridIndex(gridCursorPos)].ImageSlot.OnSelectionChanged(false);
        GameController.instance.SetUICanvas(false);
        pcObject.SetActive(false);
        GameController.instance.state = GameState.FreeRoam;
    }

    // <summary>
    // Initializes the player's pokemon party into the UI slots for the PC
    // </summary>
    private void InitializePokemonParty()
    {
        var party = PlayerController.Instance.GetComponent<PokemonParty>().Pokemons;

        for (int i = 0; i < boxPartySlots.Count; i++)
        {
            if (i < party.Count)
            {
                boxPartySlots[i].SetData(party[i]);
            }
            else
                boxPartySlots[i].ClearData();
        }
    }

    /// <summary>
    /// One-time setup function that links each slot in the UI with the corresponding
    /// BoxStorageSlotUI script. This is called once during initialization.
    /// </summary>
    private void InitializeBoxSlots()
    {
        var box = CurrentBox;

        for (int i = 0; i < boxStorageSlots.Count; i++)
        {
            var pokemon = box.GetPokemonAt(i);
            if (pokemon != null)
                boxStorageSlots[i].SetData(pokemon);
            else
                boxStorageSlots[i].ClearData();
        }
    }

    public void AddPokemonToPC(Pokemon pokemon)
    {
        storedPokemon.Add(pokemon);
    }

    // <summary>
    // Sets the pokemon in boxes just by putting 35 pokemon per box.
    // </summary>
    public void SetPokemonInBoxes(List<Pokemon> sortedList)
    {
        InitializeBoxes();
        int index = 0;

        foreach (var pokemon in sortedList)
        {
            int boxIndex = index / BOX_SIZE;
            int slotIndex = index % BOX_SIZE;

            if (boxIndex >= MAX_BOXES) break;

            boxes[boxIndex].SetPokemonAt(slotIndex, pokemon);
            index++;
        }
    }

    public void SortByTypeToBoxes()
    {
        InitializeBoxes(); // clear all boxes first

        var sortedByType = storedPokemon
            .OrderBy(p => p.Base.type1)
            .ThenBy(p => p.Base.type2)
            .ToList();

        int currentBox = 0;
        int slot = 0;
        PokemonType? lastType = null;

        foreach (var p in sortedByType)
        {
            var type = p.Base.type1;

            if (lastType == null || lastType != type || slot >= BOX_SIZE)
            {
                currentBox++;
                slot = 0;
                lastType = type;
            }

            if (currentBox >= MAX_BOXES) break;

            boxes[currentBox].SetPokemonAt(slot, p);
            slot++;
        }

        RefreshCurrentBoxUI();
    }

    /// <summary>
    /// Called whenever the currently viewed PC box is changed, or whenever
    /// the data in the current box is updated (e.g., Pokémon added/removed).
    /// This function updates all the UI slots to reflect the Pokémon in the active box.
    /// </summary>
    private void RefreshCurrentBoxUI()
    {
        var currentBox = boxes[currentBoxIndex];

        for (int i = 0; i < boxStorageSlots.Count; i++)
        {
            var slotUI = boxStorageSlots[i];
            var pokemon = currentBox.GetPokemonAt(i);

            if (pokemon != null)
                slotUI.SetData(pokemon);
            else
                slotUI.ClearData();
        }
    }
}
