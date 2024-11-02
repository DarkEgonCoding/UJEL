using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    public Pokemon Pokemon {get; set;}

    public void Setup(){
        Pokemon = new Pokemon(_base, level);
        if (isPlayerUnit){
            GetComponent<UnityEngine.UI.Image>().sprite = Pokemon.Base.BackSprite;
        }
        else{
            GetComponent<UnityEngine.UI.Image>().sprite = Pokemon.Base.FrontSprite;
        }
    }
}
