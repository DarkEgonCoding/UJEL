using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;

public class HoveredPokemonUI : MonoBehaviour
{
    [SerializeField] public Image largePokemon;
    [SerializeField] public TextMeshProUGUI pokemonName;
    [SerializeField] public TextMeshProUGUI largeLevel;
    [SerializeField] public TextMeshProUGUI type1;
    [SerializeField] public TextMeshProUGUI type2;
    [SerializeField] public TextMeshProUGUI move1;
    [SerializeField] public TextMeshProUGUI move2;
    [SerializeField] public TextMeshProUGUI move3;
    [SerializeField] public TextMeshProUGUI move4;

    public void Show(IPokemonSlotUI slotUI)
    {
        if (slotUI == null) return;
        if (PCBox.instance.IsPartyTransferMode) return;

        var pokemon = slotUI.StoredPokemon;
        if (pokemon == null)
        {
            gameObject.SetActive(false);
            return;
        }

        largePokemon.sprite = pokemon.Base.FrontSprite;
        pokemonName.text = pokemon.Base.Name;
        largeLevel.text = "Lv. " + pokemon.Level;

        move1.text = GetMoveName(pokemon, 0);
        move2.text = GetMoveName(pokemon, 1);
        move3.text = GetMoveName(pokemon, 2);
        move4.text = GetMoveName(pokemon, 3);

        type1.text = pokemon.Base.type1.ToString();
        if (pokemon.Base.type2 != PokemonType.None)
        {
            type2.text = pokemon.Base.type2.ToString();
            type2.gameObject.SetActive(true);
        }
        else
        {
            type2.gameObject.SetActive(false);
        }

        gameObject.SetActive(true);
    }

    private string GetMoveName(Pokemon pokemon, int index)
    {
        if (pokemon.Moves == null || pokemon.Moves.Count <= index)
            return "";

        var move = pokemon.Moves[index];
        if (move == null || move.Base == null)
            return "";

        return move.Base.Name;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
