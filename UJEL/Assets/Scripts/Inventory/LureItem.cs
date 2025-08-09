using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new lure")]
public class LureItem : ItemBase
{
    [SerializeField][Range(0f, 1f)] private float encounterChance;
    [SerializeField] private int duration;
    [SerializeField] bool removeLure = false;

    public override bool Use(Pokemon pokemon)
    {
        if (GameController.instance.state != GameState.FreeRoam && GameController.instance.state != GameState.Menu) return false;

        if (removeLure) // Check to see if you remove a lure / repel first
        {
            PlayerController.Instance.RemoveLure();
        }
        else if (PlayerController.Instance.lureCounter != 0)
        {
            PlayerController.Instance.lureEncounterChance = encounterChance;
            PlayerController.Instance.lureCounter = duration;
            return true;
        }

        return false;
    }
}
