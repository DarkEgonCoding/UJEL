using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongGrass : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] public float EncounterPercentage = .1f; // 10%
    [SerializeField] private EncounterZone encounterZone;

    public void OnPlayerTriggered(PlayerController player)
    {
        if (Random.value <= EncounterPercentage)
        {
            player.animator.SetBool("isMoving", false);
            GameController.instance.SetCurrentEncounterZone(encounterZone);
            player.OnEncountered.Invoke();
        }
    }
}
