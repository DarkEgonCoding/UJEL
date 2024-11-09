using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] int lettersPerSecond;
    Coroutine dialogCoroutine;

    public event Action OnShowDialog;
    public event Action OnCloseDialog;
    private string currentText;

    public static DialogManager Instance { get; private set; }
    private void Awake(){
        Instance = this;
    }

    int currentLine = 0;
    Dialog dialog;
    public bool isTyping;
    public bool skippingDialog;

    public void ShowDialog(Dialog dialog){
            OnShowDialog?.Invoke();

            this.dialog = dialog;
            dialogBox.SetActive(true);
            dialogText.text = dialog.Lines[0];
            dialogCoroutine = StartCoroutine(TypeDialog(dialog.Lines[0]));
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

    public IEnumerator SkipDialog(){
        skippingDialog = true;
        if(dialogCoroutine != null) StopCoroutine(dialogCoroutine);
        dialogText.text = currentText;
        yield return new WaitForSeconds(.2f);
        isTyping = false;
        skippingDialog = false;
    }

    public void NextDialog(){
            if(isTyping){
                if(!skippingDialog){
                    StartCoroutine(SkipDialog());
                }
            }
            else{
                ++currentLine;
                if (currentLine < dialog.Lines.Count){ 
                    dialogCoroutine = StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
                }
                else{
                    currentLine = 0;
                    dialogBox.SetActive(false);
                    OnCloseDialog?.Invoke();
                }
            }
        }
}