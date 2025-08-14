using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PokemonFoundLocationsLoader
{
    static Dictionary<string, string> foundLocations;

    public static void LoadFoundLocations()
    {
        foundLocations = new Dictionary<string, string>();

        TextAsset csvData = Resources.Load<TextAsset>("pokemon_locations");

        if (csvData == null)
        {
            Debug.LogError("Could not find pokemon_locations.csv in Resources folder!");
            return;
        }

        string[] lines = csvData.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        // Skip header
        for (int i = 1; i < lines.Length; i++)
        {
            var parts = lines[i].Split(',');

            if (parts.Length < 2)
                continue;

            string routeName = parts[0].Trim();

            // Loop over Pokémon columns (B -> last column)
            for (int j = 1; j < parts.Length; j++)
            {
                string pokemonName = parts[j].Trim();
                if (string.IsNullOrEmpty(pokemonName))
                    continue;

                pokemonName = PokemonDB.FixWeirdPokemonNames(pokemonName);

                if (foundLocations.ContainsKey(pokemonName))
                {
                    foundLocations[pokemonName] += $", {routeName}";
                }
                else
                {
                    foundLocations[pokemonName] = routeName;
                }
            }
        }

        Debug.Log($"Loaded found locations for {foundLocations.Count} Pokémon.");
        PokemonLoader.instance.FoundLocationsLoaded = true;
    }

    public static string GetLocations(string pokemonName)
    {
        pokemonName = PokemonDB.FixWeirdPokemonNames(pokemonName);

        if (foundLocations != null && foundLocations.TryGetValue(pokemonName, out string locations))
        {
            return locations;
        }

        return "???";
    }
}
