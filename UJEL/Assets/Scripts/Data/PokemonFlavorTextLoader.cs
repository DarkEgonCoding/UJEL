using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PokemonFlavorTextLoader : MonoBehaviour
{
    // Key: species_id, Value: flavor text
    private static Dictionary<int, string> flavorTexts;

    public static void LoadFlavorTexts()
    {
        flavorTexts = new Dictionary<int, string>();

        TextAsset csvData = Resources.Load<TextAsset>("pokemon_species_flavor_text");
        if (csvData == null)
        {
            Debug.LogError("Could not find pokemon_species_flavor_text.csv in Resources!");
            return;
        }

        List<string[]> rows = ParseCsv(csvData.text);

        // Skip header
        for (int i = 1; i < rows.Count; i++)
        {
            string[] parts = rows[i];
            if (parts.Length < 4) continue;

            if (!int.TryParse(parts[0], out int speciesId)) continue;
            if (!int.TryParse(parts[2], out int languageId)) continue;
            if (languageId != 9) continue; // English only

            string flavorText = parts[3].Replace("\f", " ").Replace("\n", " ").Trim();

            if (!flavorTexts.ContainsKey(speciesId))
            {
                flavorTexts.Add(speciesId, flavorText);
            }
        }

        Debug.Log($"Loaded flavor texts for {flavorTexts.Count} Pokémon.");
        PokemonLoader.instance.FlavorTextsLoaded = true;
    }

    public static string GetFlavorText(int speciesId)
    {
        if (flavorTexts != null && flavorTexts.TryGetValue(speciesId, out string text))
            return text
                .Replace("\r\n", "")
                .Replace("\r","");
        return "";
    }

    private static List<string[]> ParseCsv(string text)
    {
        List<string[]> rows = new List<string[]>();
        List<string> currentRow = new List<string>();
        StringBuilder currentField = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];

            if (c == '"')
            {
                // Double quotes inside quoted field
                if (inQuotes && i + 1 < text.Length && text[i + 1] == '"')
                {
                    currentField.Append('"');
                    i++; // skip the next quote
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                currentRow.Add(currentField.ToString());
                currentField.Clear();
            }
            else if ((c == '\n' || c == '\r') && !inQuotes)
            {
                if (c == '\r' && i + 1 < text.Length && text[i + 1] == '\n') i++; // skip \n after \r
                currentRow.Add(currentField.ToString());
                currentField.Clear();
                if (currentRow.Count > 0) rows.Add(currentRow.ToArray());
                currentRow = new List<string>();
            }
            else
            {
                currentField.Append(c);
            }
        }

        // Add last row if exists
        if (currentField.Length > 0 || currentRow.Count > 0)
        {
            currentRow.Add(currentField.ToString());
            rows.Add(currentRow.ToArray());
        }

        return rows;
    }
}
