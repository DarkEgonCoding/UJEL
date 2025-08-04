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
    public List<Pokemon> storedPokemon = new List<Pokemon>(); // This is the PC Box

    public static PCBox instance;

    [Header("Sorting")]
    [SerializeField] private SortMode currentSort = SortMode.Custom;
    [SerializeField] TextMeshProUGUI SortText;

    // Layers
    private int currentLayer = 6;
    // 0   -> Box Navigation
    // 1-5 -> Grid
    // 6   -> Sorting Layer

    const int BOX_NAVIGATION_LAYER = 0;
    const int GRID_LAYER_MIN = 1;
    const int GRID_LAYER_MAX = 5;
    const int SORTING_LAYER = 6;

    private int currentBoxIndex = 0;
    private List<string> boxNames = new List<string>
    {
        "Box 1", "Box 2", "Box 3", "Box 4", "Box 5", "Box 6", "Box 7",
        "Box 8", "Box 9", "Box 10", "Box 11", "Box 12", "Box 13",
        "Box 14", "Box 15", "Box 16", "Box 17", "Box 18", "Box 19", "Box 20"
    };

    const int GRID_COLS = 7;
    const int GRID_ROWS = 5;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) SortPC(currentSort);

        if (currentLayer >= GRID_LAYER_MIN && currentLayer <= GRID_LAYER_MAX)
        {
            //
        }
        else if (currentLayer == BOX_NAVIGATION_LAYER)
        {
            //
        }
        else if (currentLayer == SORTING_LAYER)
        {
            HandleSortMode();
        }
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
        currentLayer = newLayer;

        if (currentLayer == SORTING_LAYER)
            SortText.color = Color.blue;
        else SortText.color = Color.white;
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
}
