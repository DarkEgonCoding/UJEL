using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using PsLib;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PokemonDB
{
    static Dictionary<string, PokemonBase> pokemonsByName;
    static Dictionary<int, PokemonBase> pokemonsByDexNum;

    public static void Init(Dictionary<string, PokemonBase> pokemonsByNameVar, Dictionary<int, PokemonBase> pokemonsByDexNumVar)
    {
        pokemonsByName = pokemonsByNameVar;
        pokemonsByDexNum = pokemonsByDexNumVar;
    }

    public static PokemonBase ConvertToPokemonBase(PsLib.Dex.Pokemon p)
    {
        var pb = ScriptableObject.CreateInstance<PokemonBase>();

        pb.Init(
            num: p.num,
            name: p.name,
            types: p.types,
            hp: p.baseStats.hp,
            atk: p.baseStats.atk,
            def: p.baseStats.def,
            spa: p.baseStats.spa,
            spd: p.baseStats.spd,
            spe: p.baseStats.spe,
            abilities: p.abilities,
            heightm: p.heightm,
            weightkg: p.weightkg,
            color: p.color
        );
        return pb;
    }

    public static PokemonBase GetPokemonByName(string name)
    {
        if (!pokemonsByName.TryGetValue(name.ToLower(), out var pb))
        {
            Debug.LogError($"Pokemon '{name}' not found!");
            return null;
        }
        return pb;
    }

    public static PokemonBase GetPokemonByDexNum(int num)
    {
        if (!pokemonsByDexNum.TryGetValue(num, out var pb))
        {
            Debug.LogError($"Pokemon #{num} not found!");
            return null;
        }
        return pb;
    }
}