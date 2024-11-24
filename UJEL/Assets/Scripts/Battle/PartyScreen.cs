using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartyScreen : MonoBehaviour
{
    PartyMemberUI[] memberSlots;
    List<Pokemon> pokemons;

   public void Init(){
        if(memberSlots == null){
            memberSlots = GetComponentsInChildren<PartyMemberUI>();
        }
    }

    public void SetPartyData(List<Pokemon> pokemons){
        this.pokemons = pokemons;

        for (int i = 0; i < memberSlots.Length; i++){
            if (i < pokemons.Count){
                memberSlots[i].SetData(pokemons[i]);
                memberSlots[i].gameObject.SetActive(true);
            }
            else{
                memberSlots[i].gameObject.SetActive(false);
            }
        }
    }

    public void UpdateMemberSelection(int selectedMember){
            Debug.Log($"Pokemons.Count = {pokemons.Count}");
            Debug.Log($"MemberSlots.Count = {memberSlots.Length}");
        for (int i = 0; i < pokemons.Count; i++){
            if (i == selectedMember){
                memberSlots[i].SetSelected(true);
            }
            else{
                Debug.Log(i);
                memberSlots[i].SetSelected(false);
            }
        }
    }
}
