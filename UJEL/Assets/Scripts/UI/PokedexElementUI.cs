using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PokedexElementUI : MonoBehaviour
{
    [SerializeField] private Image pokemonImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private string descriptionText;
    [SerializeField] private string locationText;

    RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public float Height => rectTransform.rect.height;
    public Image PokemonImage => pokemonImage;
    public TextMeshProUGUI NameText => nameText;
    public TextMeshProUGUI NumberText => numberText;
    public string DescriptionText => descriptionText;
    public string LocationText => locationText;

    public void SetData(PokedexEntry entry)
    {
        numberText.text = $"#{entry.entryNumber:D3}";
        pokemonImage.sprite = entry.pokemon.FrontSprite;
        locationText = entry.pokemon.FoundLocations;

        if (entry.haveCaught)
        {
            pokemonImage.color = Color.white;
            nameText.text = entry.pokemon.PokemonName;
            descriptionText = entry.pokemon.Description;
        }
        else
        {
            pokemonImage.color = Color.black;
            nameText.text = "???";
            descriptionText = "Unknown Pokemon.";
        }
    }

    public void SetHighlighted(bool highlighted)
    {
        nameText.color = highlighted ? Color.blue : Color.black;
    }
}
