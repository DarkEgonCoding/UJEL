using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoxStorageSlotUI : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI lvlText;

    public void SetData(Pokemon pokemon)
    {
        image.sprite = pokemon.Base.FrontSprite;
        lvlText.text = "" + pokemon.Level;
        image.color = new Color(255, 255, 255, 100);
    }

    public void ClearData()
    {
        image.sprite = null;
        lvlText.text = "";
        image.color = new Color(255, 255, 255, 0);
    }
}
