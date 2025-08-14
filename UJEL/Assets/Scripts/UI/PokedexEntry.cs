using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class PokedexEntry
{
    // PokedexEntry data is SET inside the PokedexManager class when a pokedex is instantiated.
    // This is just the set of data contained inside a PokedexEntry
    public PokemonBase pokemon;
    public Sprite pokemonSprite;
    public string pokemonName;
    public string location;
    public bool haveCaught;
    public string description;
    public int entryNumber;
    public PokemonType type1;
    public PokemonType type2;
    public double heightm;
    public double weightkg;
    public string evolutionText;

    public void SetCaught(bool caught)
    {
        haveCaught = caught;
    }
}

[System.Serializable]
public class PokedexEntrySaveData
{
    public string pokemonName;
    public bool haveCaught;
}
