using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Purchasing.MiniJSON;
using System;
using System.IO;

public class LearnsetLoader : MonoBehaviour
{
    static Dictionary<string, Dictionary<string, string[]>> LearnsetByPokemonDict;

    public static void LoadLearnsets()
    {
        TextAsset rawTextAsset = Resources.Load<TextAsset>("learnsets");
        if (rawTextAsset == null)
        {
            Debug.LogError("Could not find learnsets.json in Resources folder!");
            return;
        }

        LearnsetByPokemonDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string[]>>>(rawTextAsset.text);

        DebugBulbasaurMoves();

        //PokemonLoader.instance.LearnsetsLoaded = true;
    }

    public static void DebugBulbasaurMoves()
    {
        if (LearnsetByPokemonDict == null)
        {
            Debug.LogError("Learnsets not loaded yet! Call LoadLearnsets() first.");
            return;
        }

        string targetName = "bulbasaur";
        if (!TryFindKeyIgnoreCase(LearnsetByPokemonDict, targetName, out string foundKey))
        {
            Debug.LogError($"No learnset found for '{targetName}'");
            return;
        }

        var moves = LearnsetByPokemonDict[foundKey];
        Debug.Log($"--- Learnset for {foundKey} ---");

        foreach (var moveEntry in moves)
        {
            Debug.Log($"Move: {moveEntry.Key}");
            foreach (var code in moveEntry.Value)
            {
                Debug.Log($"   {code}");
            }
        }
    }

    private static bool TryFindKeyIgnoreCase(Dictionary<string, Dictionary<string, string[]>> dict, string target, out string foundKey)
    {
        foreach (var k in dict.Keys)
        {
            if (string.Equals(k, target, StringComparison.OrdinalIgnoreCase))
            {
                foundKey = k;
                return true;
            }
        }
        foundKey = null;
        return false;
    }
}
