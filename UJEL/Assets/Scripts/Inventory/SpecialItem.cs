using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new held item")]
public class SpecialItem : ItemBase
{
    [SerializeField] public string heldItemName;

    public override bool IsReusable => true;
    public override string Name => heldItemName;

    /// <summary>
    /// Held Items in the inventory are not managed by the inventory but
    /// by the Pokemon it is used on.
    /// </summary>
    /// <param name="pokemon"></param>
    /// <returns></returns>
    public override bool Use(Pokemon pokemon)
    {
        if (GameController.instance.state != GameState.FreeRoam && GameController.instance.state != GameState.Menu) return false;

        if (pokemon == null) return false;

        pokemon.HoldItem(this);
        return true;
    }
}
