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
    public event Action onDialogFinished;
    private string currentText;

    [SerializeField] GameObject dialogBoxUI;
    [SerializeField] TextMeshProUGUI dialogTextUI;

    public static DialogManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    int currentLine = 0;
    Dialog dialog;
    public bool isTyping;
    public bool skippingDialog;
    public bool fromCutscene = false;

    public bool IsShowing { get; private set; }

    public IEnumerator ShowDialogText(string text, bool waitForInput = true)
    {
        IsShowing = true;
        dialogBoxUI.SetActive(true);

        yield return test(text);
        if (waitForInput)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        }

        dialogBoxUI.SetActive(false);
        IsShowing = false;
    }

    public void ShowDialog(Dialog dialog, Action onFinished = null, bool fromCutscene = false)
    {
        this.fromCutscene = fromCutscene;

        OnShowDialog?.Invoke();

        IsShowing = true;
        this.dialog = dialog;
        onDialogFinished = onFinished;
        dialogBox.SetActive(true);

        dialogText.text = dialog.Lines[0];
        dialogCoroutine = StartCoroutine(TypeDialog(dialog.Lines[0]));
    }

    public IEnumerator ShowDialogCoroutine(Dialog dialog)
    {
        bool isFinished = false;

        ShowDialog(dialog, () => isFinished = true, fromCutscene: true);

        yield return new WaitUntil(() => isFinished);
    }

    private IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        currentText = line;
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping = false;
    }

    private IEnumerator SkipDialog()
    {
        skippingDialog = true;
        if (dialogCoroutine != null) StopCoroutine(dialogCoroutine);
        dialogText.text = currentText;
        yield return new WaitForSeconds(.2f);
        isTyping = false;
        skippingDialog = false;
    }

    public void NextDialog()
    {
        if (isTyping)
        {
            if (!skippingDialog)
            {
                StartCoroutine(SkipDialog());
            }
        }
        else
        {
            ++currentLine;
            if (currentLine < dialog.Lines.Count)
            {
                dialogCoroutine = StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
            }
            else
            {
                currentLine = 0;
                dialogBox.SetActive(false);
                IsShowing = false;
                onDialogFinished?.Invoke();
                OnCloseDialog?.Invoke();
            }
        }
    }
    



    // Dangerous copy and pasted function -- only working for UI dialog box
    private IEnumerator test(string line)
    {
        isTyping = true;
        currentText = line;
        dialogTextUI.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogTextUI.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping = false;
    }
}