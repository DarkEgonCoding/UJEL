using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Purchasing.MiniJSON;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using Unity.VisualScripting;

public class LearnsetLoader : MonoBehaviour
{
    static Dictionary<string, PokemonLearnsetData> RawLearnsetByPokemonDict;

    public static Dictionary<string, List<MoveLearn>> LevelUpMovesByPokemonDict;

    public static void LoadLearnsets()
    {
        // Load movesets from file
        TextAsset rawTextAsset = Resources.Load<TextAsset>("learnsets");
        if (rawTextAsset == null)
        {
            Debug.LogError("Could not find learnsets.json in Resources folder!");
            return;
        }

        // Get raw data into the dictionary
        RawLearnsetByPokemonDict = JsonConvert.DeserializeObject<Dictionary<string, PokemonLearnsetData>>(rawTextAsset.text);

        LevelUpMovesByPokemonDict = new Dictionary<string, List<MoveLearn>>();

        foreach (var kvp in RawLearnsetByPokemonDict)
        {
            string pokemonId = kvp.Key;
            var learnset = kvp.Value?.learnset;
            if (learnset == null)
            {
                // Debug.LogWarning($"No learnset found for {pokemonId} — storing empty list.");
                LevelUpMovesByPokemonDict[pokemonId] = new List<MoveLearn>();
                continue;
            }

            var levelUpMoves = new List<MoveLearn>();

            foreach (var moveEntry in learnset)
            {
                string moveName = moveEntry.Key;
                List<string> methods = moveEntry.Value;
                if (methods == null) continue;

                // Pick best level across multiple 'xL#' entries by smallest
                int? bestLevel = null;

                foreach (string method in methods)
                {
                    int? parsed = ParseLevelFromMethod(method);
                    if (parsed.HasValue)
                    {
                        if (!bestLevel.HasValue || parsed.Value < bestLevel.Value)
                            bestLevel = parsed;
                    }
                }

                if (bestLevel.HasValue)
                    levelUpMoves.Add(new MoveLearn(moveName, bestLevel.Value));
            }

            // sort by level ascending
            levelUpMoves.Sort((a, b) => a.Level.CompareTo(b.Level));
            LevelUpMovesByPokemonDict[pokemonId] = levelUpMoves;
        }

        Debug.Log($"Loaded learnsets: raw entries = {RawLearnsetByPokemonDict.Count}, filtered level-up entries = {LevelUpMovesByPokemonDict.Count}");
        PokemonLoader.instance.LearnsetsLoaded = true;
    }

    /// Find the pattern "xL#"
    /// Where x is the generation #
    /// L means at Level
    /// # is the level the move is learned
    private static int? ParseLevelFromMethod(string method)
    {
        if (string.IsNullOrEmpty(method)) return null;

        method = method.Trim();

        int lIndex = method.IndexOf('L');
        if (lIndex < 0) return null; // not a level-up method

        if (lIndex == method.Length - 1)
        {
            // nothing after 'L'
            Debug.LogWarning($"Method '{method}' has no chars after 'L'");
            return null;
        }

        string after = method.Substring(lIndex + 1);

        // collect digits immediately after L
        int i = 0;
        while (i < after.Length && char.IsDigit(after[i])) i++;

        if (i == 0)
        {
            // nothing numeric after L — log diagnostic info
            Debug.LogWarning($"No digits after 'L' in method '{method}'. after='{after}'");
            // optional: show char codes to find hidden/control chars
            var codes = string.Join(", ", after.Select(c => ((int)c).ToString()));
            Debug.LogWarning($"chars after L (codes): {codes}");
            return null;
        }

        string digits = after.Substring(0, i);

        if (int.TryParse(digits, out int level))
            return level;

        Debug.LogWarning($"Could not parse digits '{digits}' from method '{method}'");
        return null;
    }
}

public class PokemonLearnsetData
{
    public Dictionary<string, List<string>> learnset { get; set; }
    public List<object> eventData { get; set; }
}

public class MoveLearn
{
    public string Move;
    public int Level;
    public MoveLearn(string move, int level) { Move = move; Level = level; }

    public override string ToString()
    {
        return $"{Move} at level {Level}";
    }
}