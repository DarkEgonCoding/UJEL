using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public static class PokemonPacker
{
    public static string Pack(Pokemon pokemon)
    {
        string packed = "";
        packed += pokemon.Base.PokemonName + '|';   // Nickname
        packed += '|';                              // Species (Matches nickname)
        packed += pokemon.HeldItem.name + '|';      // Item name (Could throw error if not matching)
        packed += pokemon.Ability + '|';            // Ability
        foreach (string move in pokemon.PMoves)     // Moves separated by commas
        {
            packed += move + ',';
        }
        packed = packed.Substring(0, packed.Length - 1);
        packed += '|';
        packed += pokemon.Nature + '|';             // Nature
        packed += '|';                              // EVs (0s if blank)
        packed += '|';                              // Gender
        packed += '|';                              // IVs (31 if blank)
        packed += '|';                              // Shiny
        packed += pokemon.Level.ToString() + '|';   // Level
        packed += '|';                              // HAPPINESS,POKEBALL,HIDDENPOWERTYPE,GIGANTAMAX,DYNAMAXLEVEL,TERATYPE
        
        return packed;
    }

    public static string Pack(PokemonParty pokemonParty)
    {
        string packed = "";
        foreach (Pokemon pokemon in pokemonParty.Pokemons)
        {
            packed += Pack(pokemon);
            packed += ']';
        }
        packed = packed.Substring(0, packed.Length - 1);

        return packed;
    }
}
