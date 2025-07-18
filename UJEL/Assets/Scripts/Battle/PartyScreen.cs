using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PartyScreen : MonoBehaviour
{
    PartyMemberUI[] memberSlots;
    List<Pokemon> pokemons;
    [SerializeField] public PartyContextMenu contextMenu;

    PokemonParty party;

    int selection = 0;

    public void Init()
    {
        if (memberSlots == null)
        {
            memberSlots = GetComponentsInChildren<PartyMemberUI>();
        }

        party = PokemonParty.GetPlayerParty();
        SetPartyData();

        party.OnUpdated += SetPartyData;
    }

    public Vector3 GetSlotPosition(int index)
    {
        return memberSlots[index].transform.position;
    }

    public void SetPartyData()
    {
        pokemons = party.Pokemons;

        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < pokemons.Count)
            {
                memberSlots[i].SetData(pokemons[i]);
                memberSlots[i].gameObject.SetActive(true);
            }
            else
            {
                memberSlots[i].gameObject.SetActive(false);
            }
        }

        UpdateMemberSelection(selection);
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i < pokemons.Count; i++)
        {
            if (i == selectedMember)
            {
                memberSlots[i].SetSelected(true);
            }
            else
            {
                memberSlots[i].SetSelected(false);
            }
        }
    }

    public void HandleUpdate(Action onSelected, Action onBack)
    {
        var prevSelection = selection;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ++selection;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            --selection;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selection -= 2;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selection += 2;
        }

        selection = Mathf.Clamp(selection, 0, pokemons.Count - 1);

        if (prevSelection != selection)
        {
            UpdateMemberSelection(selection);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            onSelected?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
        }
    }
}
