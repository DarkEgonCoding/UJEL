using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongGrass : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] public float EncounterPercentage = 10f;

    public void OnPlayerTriggered(PlayerController player){
        if (Random.Range(1, 101) <= EncounterPercentage){
                Debug.Log("Encounter");
                player.inEncounter = true;
            }
    }
}
