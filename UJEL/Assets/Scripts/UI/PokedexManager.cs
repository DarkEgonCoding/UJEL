using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

public class PokedexManager : MonoBehaviour
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
        // Load all pokemon from the Resources/Pokemons folder
        AllPokemonBase = Resources.LoadAll<PokemonBase>("Pokemons").OrderBy(p => p.PokedexNumber).ToList();
        if (AllPokemonBase.Count == 0)
        {
            Debug.LogError("No PokemonBase in Resources/Pokemons");
        }

        pokedex = new List<PokedexEntry>();
        for (int i = 0; i < AllPokemonBase.Count; i++) // Loop through all pokemon
        {
            var currPokemon = AllPokemonBase[i];
            pokedex.Add(new PokedexEntry
            {
                // Assign the data in the PokedexEntry
                pokemon = currPokemon,
                pokemonName = currPokemon.Name,
                pokemonSprite = currPokemon.FrontSprite,
                location = currPokemon.FoundLocations,
                entryNumber = i + 1,
                description = currPokemon.Description,
                haveCaught = false
            });
        }
    }
}
