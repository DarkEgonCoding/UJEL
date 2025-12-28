using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UILog : MonoBehaviour
{
    public ScrollRect scrollRect;
    [SerializeField] private TextMeshProUGUI logText;
    private string logHistory = "";

    RectTransform content;
    RectTransform viewport;

    private void Awake()
    {
        if (scrollRect == null || logText == null)
            Debug.LogError("StableSingleTextLog: assign scrollRect and logText in inspector.");

        content = scrollRect.content;
        viewport = scrollRect.viewport;

        if (content == null || viewport == null)
            Debug.LogError("StableSingleTextLog: ScrollRect must have content and viewport assigned.");

        logText.text = "";
    }

    /// <summary>
    /// Append a new line to the log without shifting the visible viewport.
    /// </summary>
    public void AddMessage(string message)
    {
        logText.text += "\n" + message;

        Canvas.ForceUpdateCanvases();

        content.sizeDelta = new Vector2(content.sizeDelta.x, logText.preferredHeight);
    }
}
