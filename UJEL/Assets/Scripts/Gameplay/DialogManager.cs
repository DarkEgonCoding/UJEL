using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] int lettersPerSecond;

    public event Action OnShowDialog;
    public event Action OnCloseDialog;

    public static DialogManager Instance { get; private set; }
    private void Awake(){
        Instance = this;
    }

    int currentLine = 0;
    Dialog dialog;
    public bool isTyping;

    public void ShowDialog(Dialog dialog){
        OnShowDialog?.Invoke();

        this.dialog = dialog;
        dialogBox.SetActive(true);
        dialogText.text = dialog.Lines[0];
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }

    public IEnumerator TypeDialog(string line){
        isTyping = true;
        dialogText.text = "";
        foreach (var letter in line.ToCharArray()){
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping = false;
    }

    public void NextDialog(){
        ++currentLine;
        if (currentLine < dialog.Lines.Count){
            StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
        }
        else{
            currentLine = 0;
            dialogBox.SetActive(false);
            OnCloseDialog?.Invoke();
        }
    }
}
