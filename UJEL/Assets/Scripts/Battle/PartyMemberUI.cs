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

    Pokemon _pokemon;

    public void SetData(Pokemon pokemon){
        _pokemon = pokemon;

        image.sprite = pokemon.Base.FrontSprite;
        nameText.text = pokemon.Base.Name;
        levelText.text = "Lvl " + pokemon.Level;
        hpBar.SetHP((float) pokemon.HP / pokemon.MaxHp);
    }
}
