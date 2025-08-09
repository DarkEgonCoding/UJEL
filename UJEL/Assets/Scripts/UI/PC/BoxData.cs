using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoxData
{
    private int BoxSize = PCBox.instance.boxSize;
    private Pokemon[] pokemons;
    public string name;

    public BoxData()
    {
        pokemons = new Pokemon[BoxSize];
        name = "";
    }

    public BoxData(string name = "")
    {
        pokemons = new Pokemon[BoxSize];
        this.name = name == "" ? "Unnamed Box" : name;
    }

    public Pokemon GetPokemonAt(int index)
    {
        if (index < 0 || index >= BoxSize) return null;
        return pokemons[index];
    }

    public void SetPokemonAt(int index, Pokemon pokemon)
    {
        if (index < 0 || index >= BoxSize) return;
        pokemons[index] = pokemon;
    }

    public void ClearPokemonAt(int index)
    {
        if (index < 0 || index >= BoxSize) return;
        pokemons[index] = null;
    }

    public Pokemon[] GetAllPokemons()
    {
        return pokemons;
    }
}
