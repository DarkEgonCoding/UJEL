using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pokemon/Encounter Zone")]
public class EncounterZone : ScriptableObject
{

    [System.Serializable]
    public class EncounterEntry
    {
        public PokemonBase baseData;
        public int minLevel = 2;
        public int maxLevel = 5;
        [Range(0f, 1f)] public float weight = 0.1f; // relative chance
    }

    [SerializeField] private List<EncounterEntry> wildEncounters;

    public Pokemon GetRandomWildPokemon()
    {
        if (wildEncounters == null || wildEncounters.Count == 0)
            return null;

        // Select based on weights
        float totalWeight = 0f;
        foreach (var entry in wildEncounters)
            totalWeight += entry.weight;

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var entry in wildEncounters)
        {
            cumulative += entry.weight;
            if (roll <= cumulative)
            {
                int level = GetLevelWithBellCurve(entry.minLevel, entry.maxLevel);
                var wildPokemon = new Pokemon(entry.baseData, level);
                return wildPokemon;
            }
        }

        Debug.LogError("Did not roll a pokemon.");
        return null;
    }

    // This uses a normalized distribution with a standard deviation sigma
    private int GetLevelWithBellCurve(int minLevel, int maxLevel)
    {
        int range = maxLevel - minLevel + 1;
        if (range <= 1)
            return minLevel;

        float mu = (minLevel + maxLevel) / 2f;
        float sigma = range / 4f; // controls spread of bell curve, tweak if needed

        // Calculate unnormalized probabilities for each level
        float[] probabilities = new float[range];
        float sum = 0f;
        for (int i = 0; i < range; i++)
        {
            float x = minLevel + i;
            float exponent = -Mathf.Pow(x - mu, 2) / (2 * sigma * sigma);
            probabilities[i] = Mathf.Exp(exponent);
            sum += probabilities[i];
        }

        // Normalize probabilities so they sum to 1
        for (int i = 0; i < range; i++)
            probabilities[i] /= sum;

        // Roll weighted random level
        float roll = Random.value;
        float cumulative = 0f;
        for (int i = 0; i < range; i++)
        {
            cumulative += probabilities[i];
            if (roll <= cumulative)
                return minLevel + i;
        }

        // Fallback (should never happen)
        return minLevel;
    }
}