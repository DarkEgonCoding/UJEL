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

    public void Show(IPokemonSlotUI slotUI)
    {
        if (slotUI == null) return;

        var pokemon = slotUI.StoredPokemon;
        if (pokemon == null)
        {
            gameObject.SetActive(false);
            return;
        }

        largePokemon.sprite = pokemon.Base.FrontSprite;
        pokemonName.text = pokemon.Base.Name;
        largeLevel.text = "Lv. " + pokemon.Level;

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

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
