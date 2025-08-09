using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConditionsDB
{
    public static void Init()
    {
        foreach (var kvp in Conditions) // kvp = key value pairs
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;

            condition.Id = conditionId;
        }
    }

    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.psn,
            new Condition()
            {
                Name = "Poison",
                StartMessage = "has been poisoned",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.ReduceHP(pokemon.MaxHp / 8);
                }
            }
        },
        {
            ConditionID.brn,
            new Condition()
            {
                Name = "Burn",
                StartMessage = "has been burned",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.ReduceHP(pokemon.MaxHp / 16);
                }
            }
        }
    };
}

public enum ConditionID
{
    none, psn, brn, slp, par, frz
}