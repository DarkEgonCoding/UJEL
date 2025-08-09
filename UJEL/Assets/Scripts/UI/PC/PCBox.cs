using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.UI;
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

public class PCBox : MonoBehaviour, ISavable
{
    [Header("PC")]
    [SerializeField] public GameObject pcObject;
    public List<Pokemon> storedPokemon = new List<Pokemon>(); // This is the PC Box
    private List<Pokemon> sortedPokemon = new List<Pokemon>();

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
    private const int MAX_BOXES = 25;
    private int currentBoxIndex = 0;
    public BoxData CurrentBox => boxes[currentBoxIndex];

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

    private float delaySwitchBox = 0.3f;
    private float lastSwitchTime = -Mathf.Infinity;
    private int partyCursor = 0;
    private Pokemon selectedPartyPokemon = null;
    private BoxPartySlotUI selectedPartySlot = null;
    private bool isPartyTransferMode = false;
    public bool IsPartyTransferMode => isPartyTransferMode;
    private bool lockGrid = false;

    private Pokemon selectedGridPokemon = null;
    private Vector2Int selectedGridPokemonPos = new Vector2Int(-1, -1);
    private bool isPCtoPartyMode = false;
    private bool lockParty = false;

    private float inputCooldown = 0.2f;
    private float inputCooldownTimer = 0f;

    const int GRID_COLS = 7;
    const int GRID_ROWS = 5;
    const int MAX_PARTY_SIZE = 6;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void InitializeBoxes(bool sortedByType = false)
    {
        boxes.Clear();

        if (!sortedByType)
        {
            for (int i = 0; i < MAX_BOXES; i++)
            {
                boxes.Add(new BoxData($"Box: {i + 1}"));
            }
            return;
        }
        else
        {
            // If sorting by type, assign names later
            for (int i = 0; i < MAX_BOXES; i++)
            {
                boxes.Add(new BoxData());
            }
        }
    }

    public void HandlePCUpdate()
    {
        if (inputCooldownTimer > 0) // Don't allow input when you just open the PC
        {
            inputCooldownTimer -= Time.deltaTime;
            return;
        }

        if (!lockGrid && !lockParty) // Don't be able to close the PC while switching Pokemon
        {
            if (Input.GetKeyDown(KeyCode.X)) ClosePCBox();
        }

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
            if (Input.GetKeyDown(KeyCode.A)) SortPC(currentSort);
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
            if (lockParty) return;

            // Go back to grid
            boxPartySlots[prevIndex].ImageSlot.OnSelectionChanged(false);
            gridCursorPos = new Vector2Int(0, 0);
            ChangeLayer(GRID_LAYER_MIN);
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            if (isPCtoPartyMode) // Lock other selections as we deal with PC -> PARTY swapping
            {
                var partyPokemon = boxPartySlots[partyCursor].StoredPokemon;
                var party = PokemonParty.GetPlayerParty();

                if (partyPokemon == null) // Moving PC pokemon to Open Slot
                {
                    // Update stored list
                    RemovePokemonFromPC(selectedGridPokemon);   // Remove from PC
                    party.AddPokemon(selectedGridPokemon);      // Add pokemon to Party

                    // Update UI
                    CurrentBox.ClearPokemonAt(GetGridIndex(selectedGridPokemonPos));    // Remove pokemon from UI
                }
                else // Swap PC and Party Pokemon
                {
                    // Update stored list
                    RemovePokemonFromPC(selectedGridPokemon);   // Remove from PC
                    AddPokemonToPC(partyPokemon);               // Add to PC

                    // This code switches the pokemon properly
                    party.Pokemons[boxPartySlots[partyCursor].PartyIndex] = selectedGridPokemon;

                    /// -------- If you ever encounter bugs with the previous line of code, use these two ------------------
                    //party.RemovePokemon(boxPartySlots[partyCursor].PartyIndex); // Remove from party
                    //party.AddPokemon(selectedGridPokemon);                      // Add to party

                    // Update UI
                    CurrentBox.SetPokemonAt(GetGridIndex(selectedGridPokemonPos), partyPokemon);    // Add party pokemon to grid
                }

                boxStorageSlots[GetGridIndex(selectedGridPokemonPos)].ImageSlot.OnSelectionChanged(false); // Remove highlight on selected pokemon

                ResetPCSelection();
                InitializePokemonParty();   // Recreate the Pokemon Party UI to fix the new pokemon
                RefreshCurrentBoxUI();      // Refresh the Box UI to match its new data
            }
            else // Able to select pokemon
            {
                SelectPartyPokemon();
            }
        }
        else return;

        // Show changes in UI highlighting
        boxPartySlots[prevIndex].ImageSlot.OnSelectionChanged(false);
        boxPartySlots[partyCursor].ImageSlot.OnSelectionChanged(true);
    }

    private void SelectPartyPokemon()
    {
        // If there is only 1 pokemon in the party, don't allow selection
        var pokemonParty = PokemonParty.GetPlayerParty();
        if (pokemonParty.Pokemons.Count <= 1)
            return;

        // If there is no pokemon in the slot, return
        var partyMon = boxPartySlots[partyCursor].StoredPokemon;
        if (partyMon == null)
            return;

        // Select the pokemon
        selectedPartyPokemon = partyMon;
        selectedPartySlot = boxPartySlots[partyCursor];
        isPartyTransferMode = true;
        lockGrid = true;

        Debug.Log($"Pokemon: {partyMon} at Party Slot {selectedPartySlot} at Party Index {selectedPartySlot.PartyIndex} was selected.");

        // Change layer to grid
        boxPartySlots[partyCursor].ImageSlot.OnSelectionChanged(false);
        gridCursorPos = new Vector2Int(0, 0);
        ChangeLayer(GRID_LAYER_MIN);
    }

    private void HandleBoxNavigation()
    {
        if (Time.time - lastSwitchTime < delaySwitchBox) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentBoxIndex = (currentBoxIndex - 1 + MAX_BOXES) % MAX_BOXES;
            UpdateBoxDisplay();
            lastSwitchTime = Time.time;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentBoxIndex = (currentBoxIndex + 1) % MAX_BOXES;
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
        boxName.text = boxes[currentBoxIndex].name;
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
                if (lockGrid) return;

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
                if (lockGrid) return;

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
                if (lockGrid) return;

                boxStorageSlots[prevIndex].ImageSlot.OnSelectionChanged(false);
                ChangeLayer(SORTING_LAYER);
                return;
            }
            gridCursorPos.y = (gridCursorPos.y + 1) % GRID_ROWS;
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            if (isPartyTransferMode) // Switching PokemonParty to Grid
            {
                var boxPokemon = CurrentBox.GetPokemonAt(GetGridIndex(gridCursorPos));
                var playerParty = PokemonParty.GetPlayerParty();

                if (selectedPartyPokemon == null || selectedPartySlot == null)
                {
                    Debug.LogWarning("No selected party Pokemon to move");
                    return;
                }

                if (boxPokemon != null) // Swap pokemon
                {
                    // Update the stored list and party pokemon
                    RemovePokemonFromPC(boxPokemon);
                    AddPokemonToPC(selectedPartyPokemon);

                    // Swap in UI
                    CurrentBox.SetPokemonAt(GetGridIndex(gridCursorPos), selectedPartyPokemon);
                    playerParty.Pokemons[selectedPartySlot.PartyIndex] = boxPokemon;
                }
                else // Move party into empty slot
                {
                    // Update stored list
                    AddPokemonToPC(selectedPartyPokemon);
                    playerParty.RemovePokemon(selectedPartySlot.PartyIndex);

                    // Update UI
                    CurrentBox.SetPokemonAt(GetGridIndex(gridCursorPos), selectedPartyPokemon);
                }

                boxPartySlots[selectedPartySlot.PartyIndex].ImageSlot.OnSelectionChanged(false); // Turn off highlight on Party

                ResetPartySelection();

                InitializePokemonParty();
                RefreshCurrentBoxUI();
            }
            else // Not in party selection mode (AKA: PC -> PARTY)
            {
                SelectGridPokemon();
            }
        }
        else return;

        UpdateGridSelection(prevIndex);
    }

    private void SelectGridPokemon()
    {
        var boxPokemon = CurrentBox.GetPokemonAt(GetGridIndex(gridCursorPos));

        if (boxPokemon == null) return;

        selectedGridPokemon = boxPokemon;
        selectedGridPokemonPos = gridCursorPos;
        isPCtoPartyMode = true;
        lockParty = true;

        Debug.Log($"Pokemon: {selectedGridPokemon} was selected.");

        // Change layer to Pokemon Party while locked
        boxPartySlots[partyCursor].ImageSlot.OnSelectionChanged(false);
        partyCursor = 0;
        ChangeLayer(PARTY_LAYER);
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
        if (currentLayer != PARTY_LAYER) hoveredPokemonUI.Show(boxStorageSlots[newIndex].ImageSlot.PokemonSlotUI);
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
            hoveredPokemonUI.Hide();
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
                sortedPokemon = storedPokemon.OrderBy(p => p.Base.Name).ToList();
                SetPokemonInBoxes(sortedPokemon);
                break;
            case SortMode.ZA:
                sortedPokemon = storedPokemon.OrderByDescending(p => p.Base.Name).ToList();
                SetPokemonInBoxes(sortedPokemon);
                break;
            case SortMode.DexNumber:
                sortedPokemon = storedPokemon.OrderBy(p => p.Base.PokedexNumber).ToList();
                SetPokemonInBoxes(sortedPokemon);
                break;
            case SortMode.Type:
                SortByTypeToBoxes();
                break;
            case SortMode.Level:
                sortedPokemon = storedPokemon.OrderByDescending(p => p.Level).ToList();
                SetPokemonInBoxes(sortedPokemon);
                break;
            case SortMode.LivingDex:
                sortedPokemon = GetLivingDexSortedList();
                SetPokemonInBoxes(sortedPokemon);
                break;
        }

        RefreshCurrentBoxUI();
        UpdateBoxDisplay();
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

    public IEnumerator OpenPCBox()
    {
        // Turn on objects
        GameController.instance.SetUICanvas(true);
        pcObject.SetActive(true);
        GameController.instance.state = GameState.PC;

        // Reset colors, text, data
        ClearAllSlots();
        ResetPartySelection();
        ResetPCSelection();
        boxName.color = Color.white;
        SortText.color = Color.white;
        currentSort = SortMode.Custom;
        UpdateSortModeUI();

        // Initialize Pokemon Party Slots and Box Storage Slots
        currentBoxIndex = 0;
        partyCursor = 0;
        InitializePokemonParty();
        SetPokemonInBoxes(storedPokemon);
        InitializeBoxSlots();

        // Reset selection and image
        currentLayer = 1;
        gridCursorPos = new Vector2Int(0, 0);
        boxStorageSlots[GetGridIndex(gridCursorPos)].ImageSlot.OnSelectionChanged(true);
        hoveredPokemonUI.Show(boxStorageSlots[GetGridIndex(gridCursorPos)].ImageSlot.PokemonSlotUI);

        inputCooldownTimer = inputCooldown;
        yield return new WaitForEndOfFrame();
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
                boxPartySlots[i].SetPartyIndex(i);
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

        UpdateBoxDisplay();
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

    /// <summary>
    /// Sorts all Pokémon in storage by primary type and distributes them across boxes.
    /// Boxes are dynamically named by type (e.g., "Box 3: Fire", "Box 4: Fire (2)").
    /// This function overrides all existing box contents and names.
    /// </summary>
    public void SortByTypeToBoxes()
    {
        // Clear all boxes and reset
        InitializeBoxes(sortedByType: true);

        // Sort all pokemon by primary type first, then secondary type
        var sortedByType = storedPokemon
            .OrderBy(p => p.Base.type1)
            .ThenBy(p => p.Base.type2)
            .ThenByDescending(p => p.Level)
            .ToList();

        int currentBox = 0;
        int slot = 0;
        PokemonType? lastType = null;

        // Tracks the # of boxes created for a type
        Dictionary<PokemonType, int> typeCounts = new Dictionary<PokemonType, int>();

        // Go through each Pokémon in the sorted list and place it in the appropriate box
        foreach (var p in sortedByType)
        {
            var type = p.Base.type1; // Get the primary type of this Pokémon

            // If we're on a new type or the current box is full, move to the next box
            if (lastType == null || lastType != type || slot >= BOX_SIZE)
            {
                if (currentBox >= MAX_BOXES) break; // Stop if we've reached the max number of available boxes

                if (!typeCounts.ContainsKey(type))
                    typeCounts[type] = 1;
                else
                    typeCounts[type]++;

                string suffix = typeCounts[type] > 1 ? $" ({typeCounts[type]})" : "";
                boxes[currentBox].name = $"Box {currentBox + 1}: {type}{suffix}";

                slot = 0;
                lastType = type;
                currentBox++;
            }

            // Place the Pokémon into the current box and move to the next slot
            if (currentBox > 0 && currentBox <= MAX_BOXES)
            {
                boxes[currentBox - 1].SetPokemonAt(slot, p);
                slot++;
            }
        }

        // Assign default names to any remaining unused boxes
        for (int i = currentBox; i < MAX_BOXES; i++)
        {
            boxes[i].name = $"Box: {i + 1}";
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

    private void ResetPartySelection()
    {
        selectedPartyPokemon = null;
        selectedPartySlot = null;
        isPartyTransferMode = false;
        lockGrid = false;
    }

    private void ResetPCSelection()
    {
        selectedGridPokemon = null;
        isPCtoPartyMode = false;
        lockParty = false;
        selectedGridPokemonPos = new Vector2Int(-1, -1);
    }

    public void RemovePokemonFromPC(Pokemon pokemon)
    {
        if (storedPokemon.Contains(pokemon))
        {
            storedPokemon.Remove(pokemon);
        }
        else
        {
            Debug.LogWarning("Tried to remove a Pokémon from PC that wasn't stored.");
        }
    }

    public object CaptureState()
    {
        var saveData = new PCSaveData
        {
            pokemons = storedPokemon
                .Where(p => p != null) // filter null pokemon
                .Select(p => p.GetSaveData())
                .ToList()
        };

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (PCSaveData)state;
        storedPokemon = saveData.pokemons
            .Where(p => p != null)
            .Select(p => new Pokemon(p))
            .ToList();
    }
}


[Serializable]
public class PCSaveData
{
    public List<PokemonSaveData> pokemons;
}
