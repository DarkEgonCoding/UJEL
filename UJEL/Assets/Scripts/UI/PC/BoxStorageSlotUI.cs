using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.Universal;

public class BoxStorageSlotUI : MonoBehaviour, IPokemonSlotUI
{
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI lvlText;
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
        image.sprite = pokemon.Base.FrontSprite;
        lvlText.text = "Lv: " + pokemon.Level;
        image.color = new Color(255, 255, 255, 100);
        storedPokemon = pokemon;
    }

    public void ClearData()
    {
        image.sprite = null;
        lvlText.text = "";
        image.color = new Color(255, 255, 255, 0);
        storedPokemon = null;
    }
}
