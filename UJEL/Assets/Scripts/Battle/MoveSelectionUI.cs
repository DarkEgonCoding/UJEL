using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoveSelectionUI : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> moveTexts;
    int currentSelection = 0;

    public void SetMoveData(List<MoveBase> currentMoves, MoveBase newMove){
        for (int i=0; i<currentMoves.Count; ++i){
            moveTexts[i].text = currentMoves[i].Name;
        }

        moveTexts[currentMoves.Count].text = newMove.Name;
    }

    [SerializeField] Color ForgetMoveHighlightColor;
    public void UpdateForgetMoveSelection(int selection){
        for (int i = 0; i < PokemonBase.MaxNumOfMoves+1; i++){
            if (i == selection){
                moveTexts[i].color = ForgetMoveHighlightColor;
            }
            else{
                moveTexts[i].color = Color.black;
            }
        }
    }
}
