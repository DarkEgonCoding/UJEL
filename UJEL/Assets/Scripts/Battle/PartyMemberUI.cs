using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI type1txt;
    [SerializeField] TextMeshProUGUI type2txt;
    [SerializeField] TextMeshProUGUI natureTxt;
    [SerializeField] Image heldItemImage;
    [SerializeField] HPBar hpBar;
    [SerializeField] Image image;
    [SerializeField] Color highlightedColor;

    Pokemon _pokemon;

    public void Init(Pokemon pokemon){
        _pokemon = pokemon;
        UpdateData();

        _pokemon.OnHPChanged += UpdateData;
    }

    void UpdateData()
    {
        image.sprite = _pokemon.Base.FrontSprite;
        nameText.text = _pokemon.Base.PokemonName;
        levelText.text = "Lvl " + _pokemon.Level;
        hpBar.SetHP((float)_pokemon.HP / _pokemon.MaxHp);
        type1txt.text = _pokemon.Base.type1.ToString();
        type2txt.text = _pokemon.Base.type2.ToString();
        natureTxt.text = $"Nature: {_pokemon.Nature.ToString()}";
        if (_pokemon.HeldItem != null && _pokemon.HeldItem.Icon != null)
        {
            heldItemImage.sprite = _pokemon.HeldItem.Icon;
            heldItemImage.enabled = true; // show only if item exists
        }
        else
        {
            heldItemImage.sprite = null;
            heldItemImage.enabled = false; // hide if no item
        }
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            nameText.faceColor = highlightedColor;
        }
        else
        {
            nameText.faceColor = Color.black;
        }
    }
}
