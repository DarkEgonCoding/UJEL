using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EvolutionsLoader
{
    static Dictionary<string, Evolution> evolutionDict;

    static Dictionary<string, List<Evolution>> stoneEvolutions = new Dictionary<string, List<Evolution>>()
    {
        {"snorunt", new List<Evolution> { new Evolution ("froslass", 0, true, EvolutionStone.Dawn)} },
        {"eevee", new List<Evolution>
            {
                new Evolution ("vaporeon", 0, true, EvolutionStone.Water),
                new Evolution("flareon", 0, true, EvolutionStone.Fire),
                new Evolution("jolteon", 0, true, EvolutionStone.Thunder),
                new Evolution("espeon", 0, true, EvolutionStone.Dawn),
                new Evolution("umbreon", 0, true, EvolutionStone.Dusk),
                new Evolution("leafeon", 0, true, EvolutionStone.Leaf),
                new Evolution("glaceon", 0, true, EvolutionStone.Ice),
                new Evolution("sylveon", 0, true, EvolutionStone.Shiny)
            }
        },
        {"vulpix", new List<Evolution> { new Evolution ("ninetales", 0, true, EvolutionStone.Fire)} },
        {"vulpixalola", new List<Evolution> { new Evolution ("ninetalesalola", 0, true, EvolutionStone.Ice)} },
        {"kirlia", new List<Evolution> { new Evolution ("gallade", 0, true, EvolutionStone.Dawn)} },
        {"charjabug", new List<Evolution> { new Evolution ("vikavolt", 0, true, EvolutionStone.Thunder)} },
        {"helioptile", new List<Evolution> { new Evolution ("heliolisk", 0, true, EvolutionStone.Sun)} },
        {"lampent", new List<Evolution> { new Evolution ("chandelure", 0, true, EvolutionStone.Dusk)} },
        {"nuzleaf", new List<Evolution> { new Evolution ("shiftry", 0, true, EvolutionStone.Leaf)} },
        {"mantyke", new List<Evolution> { new Evolution ("mantine", 0, true, EvolutionStone.Water)} },
        {"eelektrik", new List<Evolution> { new Evolution ("eelektross", 0, true, EvolutionStone.Thunder)} },
        {"electabuzz", new List<Evolution> { new Evolution ("electivire", 0, true, EvolutionStone.Thunder)} },
        {"pikachu", new List<Evolution> { new Evolution ("raichu", 0, true, EvolutionStone.Thunder)} },
        {"magneton", new List<Evolution> { new Evolution ("magnezone", 0, true, EvolutionStone.Thunder)} },
        {"shellder", new List<Evolution> { new Evolution ("cloyster", 0, true, EvolutionStone.Water)} },
        {"lotad", new List<Evolution> { new Evolution ("lombre", 0, true, EvolutionStone.Water)} },
        {"doublade", new List<Evolution> { new Evolution ("aegislash", 0, true, EvolutionStone.Dusk)} },
        {"nincada", new List<Evolution> { new Evolution ("shedinja", 0, true, EvolutionStone.Dusk)}}
    };

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
    public static List<Evolution> GetEvolution(string basePokemonName)
    {
        if (evolutionDict == null || stoneEvolutions == null)
        {
            Debug.LogError("Evolution dictionary is empty.");
            return null;
        }

        List<Evolution> pEvolutions = new List<Evolution>();

        // Level evolutions
        if (evolutionDict.TryGetValue(basePokemonName, out Evolution evo))
        {
            pEvolutions.Add(evo);
        }
        else
        {
            // Debug.Log($"No level evolution found for '{basePokemonName}'");
        }

        if (stoneEvolutions.TryGetValue(PokemonDB.FixWeirdPokemonNames(basePokemonName), out List<Evolution> stoneEvos))
        {
            pEvolutions.AddRange(stoneEvos);
        }
        else
        {
            // Debug.Log($"No stone evolution found for '{basePokemonName}'");
        }

        return pEvolutions;
    }
}
