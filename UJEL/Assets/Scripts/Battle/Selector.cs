using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.EditorTools;
using UnityEngine.Events;

public class Selector : MonoBehaviour
{
    [Header("Menu Items")]
    [SerializeField] List<TextMeshProUGUI> menuItems;

    [Header("Layout Settings")]
    [SerializeField] private bool isHorizontal = false; // false = vertical (default), true = horizontal

    [Header("Visual Settings")]
    [SerializeField] public Color normalColor = Color.white;
    [SerializeField] Color highlightColor = Color.blue;

    [SerializeField] private UnityEvent[] onSelectEvents;
    [SerializeField] private UnityEvent onReturn;

    private int selectedIndex = 0;

    public void MenuUp()
    {
        if (!isHorizontal)
        {
            selectedIndex = (selectedIndex - 1 + menuItems.Count) % menuItems.Count;
            UpdateMenuVisual();
        }
    }

    public void MenuDown()
    {
        if (!isHorizontal)
        {
            selectedIndex = (selectedIndex + 1) % menuItems.Count;
            UpdateMenuVisual();
        }
    }

    public void MenuLeft()
    {
        Debug.Log("Test");
        if (isHorizontal)
        {
            selectedIndex = (selectedIndex + 1) % menuItems.Count;
            UpdateMenuVisual();
        }
    }

    public void MenuRight()
    {
        Debug.Log("Test");
        if (isHorizontal)
        {
            selectedIndex = (selectedIndex - 1 + menuItems.Count) % menuItems.Count;
            UpdateMenuVisual();
        }
    }

    public void SelectItem()
    {
        if (selectedIndex < onSelectEvents.Length)
        {
            onSelectEvents[selectedIndex]?.Invoke();
        }
        else
        {
            Debug.LogWarning("No UnityEvent assigned to selected index.");
        }
    }

    public void Return()
    {
        onReturn.Invoke();
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
