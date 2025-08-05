using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoxPartySlotUI : MonoBehaviour, IPokemonSlotUI
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI lvlText;
    [SerializeField] Image image;
    Pokemon storedPokemon;

    ImageSlot imageSlot;

    public Pokemon StoredPokemon => storedPokemon;
    public ImageSlot ImageSlot => imageSlot;

    public void Awake()
    {
        if (imageSlot == null) imageSlot = gameObject.GetComponent<ImageSlot>();
    }

    public void SetData(Pokemon pokemon)
    {
        nameText.text = pokemon.Base.name;
        lvlText.text = "Lv: " + pokemon.Level;
        image.sprite = pokemon.Base.FrontSprite;
        image.color = new Color(255, 255, 255, 100);
        storedPokemon = pokemon;
    }

    public void ClearData()
    {
        nameText.text = "";
        lvlText.text = "";
        image.sprite = null;
        image.color = new Color(255, 255, 255, 0);
        storedPokemon = null;
    }
}
