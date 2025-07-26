using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
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

    [Header("Status")]
    [SerializeField] ConditionID status;
    [SerializeField] bool recoverAllStatus;

    [Header("Revive")]
    [SerializeField] bool revive;
    [SerializeField] bool maxRevive;

    public override bool Use(Pokemon pokemon)
    {
        // Revive
        if (revive || maxRevive)
        {
            if (pokemon.HP > 0) return false; // Pokemon is not fainted

            if (revive)
            {
                pokemon.IncreaseHP(pokemon.MaxHp / 2);
            }
            else if (maxRevive)
            {
                pokemon.IncreaseHP(pokemon.MaxHp);
            }

            // Cure status condition here ***FOR LATER***

            return true;
        }

        if (pokemon.HP == 0) return false; // Can't use item if pokemon is fainted

        // Restore HP
        if (hpAmount > 0) // If HP is greater than 0
        {
            if (pokemon.HP == pokemon.MaxHp) // If at max hp
            {
                return false;
            }

            if (restoreMaxHP)
            {
                pokemon.IncreaseHP(pokemon.MaxHp);
            }
            else
            {
                pokemon.IncreaseHP(hpAmount);
            }
        }

        // Recover Status
        if (recoverAllStatus || status != ConditionID.none)
        {
            if (pokemon.Status == null)
            {
                return false;
            }

            if (recoverAllStatus)
            {
                pokemon.CureStatus();
            }
            else
            {
                if (pokemon.Status.Id == status)
                {
                    pokemon.CureStatus();
                }
                else return false;
            }
        }

        // Restore PP
        if (restoreMaxPP)
        {
            if (restoreAllMoves) pokemon.Moves.ForEach(m => m.IncreasePP(m.Base.PP));
            else return false; // FIX: RESTORE 1 MOVE
        }
        else if (ppAmount > 0)
        {
            if (restoreAllMoves) pokemon.Moves.ForEach(m => m.IncreasePP(ppAmount));
            else return false; // FIX: RESTORE 1 MOVE
        }

        return true;
    }
}
