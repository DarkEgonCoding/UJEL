using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new recovery item")]
public class RecoveryItem : ItemBase
{
    [Header("HP")]
    [SerializeField] int hpAmount;
    [SerializeField] bool restoreMaxHP;

    [Header("PP")]
    [SerializeField] int ppAmount;
    [SerializeField] bool restoreAllMoves;
    [SerializeField] bool restoreMaxPP;

    // Status positions

    [Header("Revive")]
    [SerializeField] bool revive;
    [SerializeField] bool maxRevive;

    public override bool Use(Pokemon pokemon)
    {
        if (hpAmount > 0)
        {
            if (pokemon.HP == pokemon.MaxHp)
            {
                return false;
            }

            // Update HP here
            return false;
        }

        return false;
    }
}
