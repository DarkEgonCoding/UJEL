using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class PokemonDB
{
    static Dictionary<string, PokemonBase> pokemons;

    TextAsset jsonFile;
    //Dictionary<string, PokemonData> pokedex;

    public static void Init()
    {
        pokemons = new Dictionary<string, PokemonBase>();

        var pokemonArray = Resources.LoadAll<PokemonBase>("");
        foreach (var pokemon in pokemonArray)
        {
            if (pokemons.ContainsKey(pokemon.Name))
            {
                Debug.LogError($"There are two pokemons with the name {pokemon.Name}");
                continue;
            }

            pokemons[pokemon.Name] = pokemon;
        }

        //LoadFromJson();
    }

    public static PokemonBase GetPokemonByName(string name)
    {
        if (!pokemons.ContainsKey(name))
        {
            Debug.LogError($"Pokemon with name {name} not found in the database");
            return null;
        }

        return pokemons[name];
    }

    private void LoadFromJson()
    {
        jsonFile = Resources.Load<TextAsset>("pokedex");
        string jsonText = jsonFile.text;
    }
}
/*
[System.Serializable]
public class Wrapper
{
    public List<PokemonEntry> entries;

    public Dictionary<string, PokemonData> ToDictionary()
    {
        return entries.ToDictionary(e => e.key, e => e.data);
    }
}

[System.Serializable]
public class PokemonEntry {
    public string key;
    public PokemonData data;
}
*/
