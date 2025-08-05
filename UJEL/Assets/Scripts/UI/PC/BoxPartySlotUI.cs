using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoxPartySlotUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI lvlText;
    [SerializeField] Image image;

    public void SetData(Pokemon pokemon)
    {
        nameText.text = pokemon.Base.name;
        lvlText.text = "" + pokemon.Level;
        image.sprite = pokemon.Base.FrontSprite;
        image.color = new Color(255, 255, 255, 100);
    }

    public void ClearData()
    {
        nameText.text = "";
        lvlText.text = "";
        image.sprite = null;
        image.color = new Color(255, 255, 255, 0);
    }
}
