using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CatchRatesLoader
{
    static Dictionary<int, int> catchRatesByDexNum;

    public static void LoadCatchRates()
    {
        catchRatesByDexNum = new Dictionary<int, int>();

        TextAsset csvData = Resources.Load<TextAsset>("catch_rates"); // no .csv extension
        if (csvData == null)
        {
            Debug.LogError("Could not find catch_rates.csv in Resources folder!");
            return;
        }

        // Split entire CSV content by newlines, handle Windows/Mac/Linux line endings
        string[] lines = csvData.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        // Skip header, start at line 1
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            var parts = line.Split(',');

            if (parts.Length < 4) continue; // Make sure columns exist

            // Parse Dex number
            if (int.TryParse(parts[0], out int dexNum))
            {
                string catchRateStr = parts[3].Trim();

                if (int.TryParse(catchRateStr, out int catchRate))
                {
                    catchRatesByDexNum[dexNum] = catchRate;

                    if (catchRatesByDexNum[dexNum] < 3 || catchRatesByDexNum[dexNum] > 255)
                    {
                        Debug.LogWarning($"Catch rate outside of range '{catchRateStr}' for Dex #{dexNum}'");
                    }
                }
                else
                {
                    Debug.LogWarning($"Invalid catch rate '{catchRateStr}' for Dex #{dexNum}");
                }
            }
            else
            {
                Debug.LogWarning($"Invalid Dex number '{parts[0]}' on line {i + 1}");
            }
        }

        Debug.Log($"Loaded {catchRatesByDexNum.Count} catch rates.");
        PokemonLoader.instance.CatchRatesLoaded = true;
    }

    public static int GetCatchRate(int dexNum)
    {
        if (catchRatesByDexNum != null && catchRatesByDexNum.TryGetValue(dexNum, out int rate))
            return rate;

        Debug.LogWarning($"Catch rate not found for Dex #{dexNum}, returning default 200.");
        return 200; // default catch rate fallback
    }
}
