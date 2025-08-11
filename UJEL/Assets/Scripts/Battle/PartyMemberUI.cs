using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
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
        hpBar.SetHP((float) _pokemon.HP / _pokemon.MaxHp);
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
