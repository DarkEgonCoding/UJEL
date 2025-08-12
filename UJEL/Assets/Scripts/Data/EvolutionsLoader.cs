using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EvolutionsLoader
{
    static Dictionary<string, Evolution> evolutionDict;

    public static void LoadEvolutions()
    {
        evolutionDict = new Dictionary<string, Evolution>();

        TextAsset csvData = Resources.Load<TextAsset>("evolutions_by_level");
        if (csvData == null)
        {
            Debug.LogError("Could not find evolutions.csv in Resources folder!");
            return;
        }

        string[] lines = csvData.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        // Skip header line
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            var parts = line.Split(',');

            if (parts.Length < 3) continue;

            string baseName = parts[0].Trim();
            string evolvesInto = parts[1].Trim();
            if (!int.TryParse(parts[2].Trim(), out int requiredLevel))
            {
                Debug.LogWarning($"Could not parse required level for evolution line: {line}");
                continue;
            }

            if (!evolutionDict.ContainsKey(baseName))
            {
                evolutionDict.Add(baseName, new Evolution(evolvesInto, requiredLevel));
            }
            else
            {
                Debug.LogWarning($"Duplicate evolution entry for {baseName} in evolutions.csv");
            }
        }

        Debug.Log($"Loaded {evolutionDict.Count} evolutions.");
        PokemonLoader.instance.EvolutionsLoaded = true;
    }

    // Get evolution for a base Pokémon, or null if none
    public static Evolution GetEvolution(string basePokemonName)
    {
        if (evolutionDict != null && evolutionDict.TryGetValue(basePokemonName, out Evolution evo))
        {
            return evo;
        }
        else
        {
            Debug.Log($"No evolution found for '{basePokemonName}'");
            return null;
        }
    }
}
