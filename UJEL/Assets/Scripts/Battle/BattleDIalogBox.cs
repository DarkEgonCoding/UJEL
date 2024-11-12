using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] int lettersPerSecond = 30;
    [SerializeField] Color highlightedColor;
    [SerializeField] public GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;
    [SerializeField] List<TextMeshProUGUI> actionTexts;
    [SerializeField] List<TextMeshProUGUI> moveTexts;
    [SerializeField] TextMeshProUGUI ppText;
    [SerializeField] TextMeshProUGUI typeText;
    bool skippingDialog;
    public bool isTyping;
    Coroutine dialogCoroutine;
    string currentText;
    
    public void SetDialog(string dialog){
        dialogText.text = dialog;
    }

    public Coroutine StartDialog(string line){
        dialogCoroutine = StartCoroutine(TypeDialog(line));
        return dialogCoroutine;
    }

    private IEnumerator TypeDialog(string line){
        isTyping = true;
        currentText = line;
        dialogText.text = "";
        foreach (var letter in line.ToCharArray()){
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping = false;
    }

    private IEnumerator SkipDialog(){
        skippingDialog = true;
        if(dialogCoroutine != null) StopCoroutine(dialogCoroutine);
        dialogText.text = currentText;
        yield return new WaitForSeconds(.2f);
        isTyping = false;
        skippingDialog = false;
    }

    public void TrySkipDialog(){
        Debug.Log("Skip");
        if(isTyping)
        {
            if(!skippingDialog)
            {
                StartCoroutine(SkipDialog());
            }
        }
    }

    public void EnableDialogText(bool enabled){
        dialogText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled){
        actionSelector.SetActive(enabled);
    }

    public void EnableMoveSelector(bool enabled){
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }

    public void UpdateActionSelection(int selectedAction){
        for (int i=0; i<actionTexts.Count; ++i){
            if (i == selectedAction){
                actionTexts[i].color = highlightedColor;
            }
            else{
                actionTexts[i].color = Color.black;
            }
        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move){
        for (int i=0; i<moveTexts.Count; ++i){
            if (i == selectedMove){
                moveTexts[i].color = highlightedColor;
            }
            else{
                moveTexts[i].color = Color.black;
            }
        }

        ppText.text = $"PP {move.PP}/{move.Base.PP}";
        typeText.text = move.Base.Type.ToString();
    }

    public void SetMoveNames(List<Move> moves){
        for (int i=0; i<moveTexts.Count; ++i){
            if (i < moves.Count){
                moveTexts[i].text = moves[i].Base.Name;
            }
            else {
                moveTexts[i].text = "-";
            }
        }
    }
}
