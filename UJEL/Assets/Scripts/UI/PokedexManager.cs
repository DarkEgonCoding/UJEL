using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

public class PokedexManager : MonoBehaviour, ISavable
{
    List<PokedexEntry> pokedex;

    public static PokedexManager instance;

    // List of all pokemon (can be assigned manually or make a script later)
    List<PokemonBase> AllPokemonBase;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public List<PokedexEntry> Pokedex => pokedex;

    public void Start()
    {
        LoadAllPokemon();
        InitializePokedex();
    }

    public void LoadAllPokemon()
    {
        // Load all pokemon from the Resources/Pokemons folder
        AllPokemonBase = Resources.LoadAll<PokemonBase>("Pokemons").OrderBy(p => p.PokedexNumber).ToList();
        if (AllPokemonBase.Count == 0)
        {
            Debug.LogError("No PokemonBase in Resources/Pokemons");
        }
    }

    public void InitializePokedex()
    {
        pokedex = new List<PokedexEntry>();
        for (int i = 0; i < AllPokemonBase.Count; i++) // Loop through all pokemon
        {
            var currPokemon = AllPokemonBase[i];
            pokedex.Add(new PokedexEntry
            {
                // Assign the data in the PokedexEntry
                pokemon = currPokemon,
                pokemonName = currPokemon.PokemonName,
                pokemonSprite = currPokemon.FrontSprite,
                location = currPokemon.FoundLocations,
                entryNumber = i + 1,
                description = currPokemon.Description,
                haveCaught = false
            });
        }
    }

    public void SetCaughtStatus(PokemonBase pokemon, bool caught)
    {
        int index = pokedex.FindIndex(e => e.pokemon == pokemon);
        if (index != -1)
        {
            pokedex[index].SetCaught(caught);

            // If UI exists and is initialized, update it
            if (PokedexUIManager.instance != null && index < PokedexUIManager.instance.PokedexUIList.Count)
            {
                PokedexUIManager.instance.PokedexUIList[index].SetData(pokedex[index]);
            }
        }
    }

    public object CaptureState()
    {
        var saveData = new PokedexSaveData
        {
            entries = pokedex.Select(entry => new PokedexEntrySaveData
            {
                pokemonName = entry.pokemonName,
                haveCaught = entry.haveCaught
            }).ToList()
        };

        return saveData;
    }

    public void RestoreState(object state)
    {
        Debug.Log("Loading pokedex data");
        var saveData = state as PokedexSaveData;
        if (saveData == null)
        {
            Debug.LogWarning("Failed to restore pokedex: save data is null.");
            return;
        }

        LoadAllPokemon();
        InitializePokedex();

        foreach (var entrySave in saveData.entries)
        {
            // Lambda expression: goes through the pokedex and finds an "e" entry that has a pokemonName equal to the entrySave pokemonName.
            var match = pokedex.FirstOrDefault(e => e.pokemonName == entrySave.pokemonName);
            if (match != null)
            {
                match.haveCaught = entrySave.haveCaught;
            }
        }
    }
}

[System.Serializable]
public class PokedexSaveData
{
    public List<PokedexEntrySaveData> entries;
}
