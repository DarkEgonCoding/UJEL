using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new evolution stone")]
public class EvolutionItem : ItemBase
{
    [SerializeField] EvolutionStone evolutionStone;

    public override bool Use(Pokemon pokemon)
    {
        if (GameController.instance.state != GameState.FreeRoam && GameController.instance.state != GameState.Menu) return false;

        Evolution matchingEvolution = pokemon.Base.Evolutions.FirstOrDefault(e => e.NeedsStone && e.RequiredStone == evolutionStone);

        if (matchingEvolution != null)
        {
            pokemon.Evolve(matchingEvolution);
            return true;
        }

        return false;
    }
}
