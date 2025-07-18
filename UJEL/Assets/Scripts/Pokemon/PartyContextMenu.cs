using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;

public class PartyContextMenu : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> options;
    [SerializeField] Color selectedColor;
    [SerializeField] Color defaultColor;

    int selectedOption = 0;
    Action<int> onSelected;

    public void Show(Vector3 position, Action<int> onOptionSelected)
    {
        gameObject.SetActive(true);
        transform.position = position;
        selectedOption = 0;
        UpdateOptionHighlight();
        onSelected = onOptionSelected;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void HandleUpdate(string pressed)
    {
        if (pressed == "down") // replace with input system if needed
        {
            selectedOption = (selectedOption + 1) % options.Count;
            UpdateOptionHighlight();
        }
        else if (pressed == "up")
        {
            selectedOption = (selectedOption - 1 + options.Count) % options.Count;
            UpdateOptionHighlight();
        }
        else if (pressed == "z") // Confirm
        {
            onSelected?.Invoke(selectedOption);
        }
        else if (pressed == "x") // Cancel
        {
            onSelected?.Invoke(options.Count - 1); // assume last option is "Cancel"
        }
    }

    void UpdateOptionHighlight()
    {
        for (int i = 0; i < options.Count; i++)
        {
            options[i].color = (i == selectedOption) ? selectedColor : defaultColor;
        }
    }
}
