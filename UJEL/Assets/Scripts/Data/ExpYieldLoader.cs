using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpYieldLoader : MonoBehaviour
{
    static Dictionary<string, int> xpYields;

    public static void LoadXPYields()
    {
        xpYields = new Dictionary<string, int>();

        TextAsset csvData = Resources.Load<TextAsset>("pokemon_exp_yield");
        if (csvData == null)
        {
            Debug.LogError("Could not find pokemon_xp_yield.csv in Resources folder!");
            return;
        }

        string[] lines = csvData.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        // Skip header (line 0)
        for (int i = 1; i < lines.Length; i++)
        {
            var parts = lines[i].Split(',');

            if (parts.Length < 3)
            {
                Debug.LogWarning($"Skipping malformed line: {lines[i]}");
                continue;
            }

            string name = parts[1].Trim();
            name = PokemonDB.FixWeirdPokemonNames(name);

            if (int.TryParse(parts[2].Trim(), out int xp))
            {
                if (!xpYields.ContainsKey(name))
                    xpYields.Add(name, xp);
                else
                    Debug.LogWarning($"Duplicate XP entry for {name} in CSV.");
            }
            else
            {
                Debug.LogWarning($"Failed to parse XP for line: {lines[i]}");
            }
        }

        Debug.Log($"Loaded XP yields for {xpYields.Count} Pokémon.");
        PokemonLoader.instance.ExpYieldLoaded = true;
    }

    public static int GetXPYield(string pokemonName)
    {
        pokemonName = PokemonDB.FixWeirdPokemonNames(pokemonName);

        if (xpYields != null && xpYields.TryGetValue(pokemonName, out int xp))
            return xp;
        else
            return 0; // Default if not found
    }
}
