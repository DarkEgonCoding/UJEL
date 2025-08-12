using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PokedexNumberLoader
{
    public static Dictionary<string, int> pokemonNameToDexNum;
    public static Dictionary<int, string> dexNumToPokemonName;

    public static List<string> PJEL_Pokedex_Pokemon;

    public static void LoadDexNumbers()
    {
        pokemonNameToDexNum = new Dictionary<string, int>();
        dexNumToPokemonName = new Dictionary<int, string>();

        PJEL_Pokedex_Pokemon = new List<string>();

        TextAsset csvData = Resources.Load<TextAsset>("dex_numbers");

        if (csvData == null)
        {
            Debug.LogError("Could not find pokedex_numbers.csv in Resources folder!");
            return;
        }

        string[] lines = csvData.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        // Skip header (line 0)
        for (int i = 1; i < lines.Length; i++)
        {
            var parts = lines[i].Split(',');

            if (parts.Length < 2)
                continue;

            string name = parts[0].Trim();
            name = FixOddPokemonNames(name);

            PJEL_Pokedex_Pokemon.Add(name);
            //Debug.Log(name);

            if (int.TryParse(parts[1].Trim(), out int dexNum))
            {
                if (!pokemonNameToDexNum.ContainsKey(name))
                    pokemonNameToDexNum.Add(name, dexNum);

                if (!dexNumToPokemonName.ContainsKey(dexNum))
                    dexNumToPokemonName.Add(dexNum, name);
            }
            else
            {
                Debug.LogWarning($"Failed to parse Dex Num for line: {lines[i]}");
            }
        }

        Debug.Log($"Loaded {pokemonNameToDexNum.Count} Pokédex entries.");
        PokemonLoader.instance.DexNumbersLoaded = true;
    }

    public static int GetDexNumber(string pokemonName, bool DebugError = true)
    {
        if (pokemonNameToDexNum != null && pokemonNameToDexNum.TryGetValue(pokemonName, out int dex))
            return dex;

        if(DebugError) Debug.LogWarning($"Dex number not found for {pokemonName}");
        return -1;
    }

    public static string GetPokemonName(int dexNum)
    {
        if (dexNumToPokemonName != null && dexNumToPokemonName.TryGetValue(dexNum, out string name))
            return name;

        Debug.LogWarning($"Pokémon name not found for Dex #{dexNum}");
        return null;
    }
    
    private static string FixOddPokemonNames(string pokemonName)
    {
        pokemonName = pokemonName.ToLower().Replace(" ", "").Replace("-", "").Replace(".", "").Replace("é", "e")
        .Replace("’", "'")   // U+2019 right single quote
        .Replace("‘", "'")   // U+2018 left single quote
        .Replace("‛", "'")   // U+201B single high-reversed-9 quote
        .Replace("′", "'");   // U+2032 prime (just in case)
        

        if (pokemonName == "kommo-o") return "kommoo";
        else if (pokemonName == "hakamo-o") return "hakamoo";
        else if (pokemonName == "farfetch'd") return "farfetchd";
        else if (pokemonName == "ho-oh") return "hooh";
        else if (pokemonName == "porygon-z") return "porygonz";
        else if (pokemonName == "jangmo-o") return "jangmoo";
        else if (pokemonName == "sirfetch'd") return "sirfetchd";

        return pokemonName;
    }
}
