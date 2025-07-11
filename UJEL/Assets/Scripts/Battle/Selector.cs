using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.EditorTools;
using UnityEngine.Events;

public class Selector : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> menuItems;
    [SerializeField] public Color normalColor = Color.white;
    [SerializeField] Color highlightColor = Color.blue;
    [SerializeField] int menuIndex = 0;

    [SerializeField] private UnityEvent[] onSelectEvents;

    private int selectedIndex = 0;

    public void MenuUp()
    {
        selectedIndex = (selectedIndex - 1 + menuItems.Count) % menuItems.Count;
        UpdateMenuVisual();
    }

    public void MenuDown()
    {
        selectedIndex = (selectedIndex + 1) % menuItems.Count;
        UpdateMenuVisual();
    }

    public void SelectItem()
    {
        Debug.Log("Selector: SelectItem called on index " + selectedIndex);

        if (selectedIndex < onSelectEvents.Length)
        {
            onSelectEvents[selectedIndex]?.Invoke();
        }
        else
        {
            Debug.LogWarning("No UnityEvent assigned to selected index.");
        }
    }
    
    public void UpdateMenuVisual()
    {
        for (int i = 0; i < menuItems.Count; i++)
        {
            if (i == selectedIndex)
            {
                menuItems[i].color = highlightColor;
                menuItems[i].fontStyle = FontStyles.Bold;
            }
            else
            {
                menuItems[i].color = normalColor;
                menuItems[i].fontStyle = FontStyles.Normal;
            }
        }
    }
}
