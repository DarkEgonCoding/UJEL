using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongGrass : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] public float EncounterPercentage = .077f; // 1 in 13 tiles
    [SerializeField] private EncounterZone encounterZone;

    public void OnPlayerTriggered(PlayerController player)
    {
        var EncounterChance = (encounterZone != null && encounterZone.EncounterRate != 0) ? encounterZone.EncounterRate : EncounterPercentage;

        if (player.lureCounter > 0 && player.lureEncounterChance >= 0) // Ensure a lure / repel is active
        {
            EncounterChance = player.lureEncounterChance;
        }

        if (Random.Range(0, 10000) == 0 && GameController.instance.legendaryEncounter != null) // On 1 in 10,000 chance make the encounter a legendary pokemon
        {
            player.animator.SetBool("isMoving", false);
            GameController.instance.SetCurrentEncounterZone(GameController.instance.legendaryEncounter);
            player.OnEncountered.Invoke();
        }
        else if (Random.value <= EncounterChance)
        {
            player.animator.SetBool("isMoving", false);
            GameController.instance.SetCurrentEncounterZone(encounterZone);
            player.OnEncountered.Invoke();
        }
    }
}
