using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class GrowthRateLoader
{
    static Dictionary<int, GrowthRate> growthRatesByDexNum;

    public static void LoadGrowthRates()
    {
        growthRatesByDexNum = new Dictionary<int, GrowthRate>();

        TextAsset csvData = Resources.Load<TextAsset>("growth_rates"); // no .csv extension
        if (csvData == null)
        {
            Debug.LogError("Could not find growth_rates.csv in Resources folder!");
            return;
        }

        // Split entire CSV content by newlines
        string[] lines = csvData.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        // Start from 1 to skip header
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            var parts = line.Split(',');


            if (parts.Length < 4) continue; // make sure columns exist

            // Parse Dex number
            if (int.TryParse(parts[0], out int dexNum))
            {
                string growthRateStr = parts[3].Trim();

                if (TryParseGrowthRate(growthRateStr, out GrowthRate growthRate))
                {
                    growthRatesByDexNum[dexNum] = growthRate;
                }
                else
                {
                    Debug.LogWarning($"Unknown growth rate '{growthRateStr}' for Dex #{dexNum}");
                }
            }
        }

        Debug.Log($"Loaded {growthRatesByDexNum.Count} growth rates.");
        PokemonLoader.instance.GrowthRatesLoaded = true;
    }

    public static bool TryParseGrowthRate(string text, out GrowthRate growthRate)
    {
        switch (text.ToLower())
        {
            case "medium fast":
                growthRate = GrowthRate.MediumFast; return true;
            case "erratic":
                growthRate = GrowthRate.Erratic; return true;
            case "fluctuating":
                growthRate = GrowthRate.Fluctuating; return true;
            case "medium slow":
                growthRate = GrowthRate.MediumSlow; return true;
            case "fast":
                growthRate = GrowthRate.Fast; return true;
            case "slow":
                growthRate = GrowthRate.Slow; return true;
            default:
                growthRate = GrowthRate.MediumFast;
                return false;
        }
    }

    public static GrowthRate GetGrowthRate(int dexNum)
    {
        if (growthRatesByDexNum != null && growthRatesByDexNum.TryGetValue(dexNum, out GrowthRate rate))
            return rate;

        Debug.LogWarning("Growth Rate not found, returning MediumFast");
        return GrowthRate.MediumFast; // fallback default
    }
}
