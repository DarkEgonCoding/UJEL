using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Purchasing.MiniJSON;
using System;
using System.IO;

public class LearnsetLoader : MonoBehaviour
{
    static Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> LearnsetByPokemonDict;

    public static void LoadLearnsets()
    {
        TextAsset rawTextAsset = Resources.Load<TextAsset>("learnsets");
        if (rawTextAsset == null)
        {
            Debug.LogError("Could not find learnsets.json in Resources folder!");
            return;
        }

        LearnsetByPokemonDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>>(rawTextAsset.text);

        foreach (KeyValuePair<string, List<string>> keyValuePair in LearnsetByPokemonDict["pikachu"]["learnset"])
        {
            Debug.Log(keyValuePair.Key);
        }
        
        //PokemonLoader.instance.LearnsetsLoaded = true;
    }
}
