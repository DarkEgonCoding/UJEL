using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PokemonInGame))]
public class PokemonInGameDrawer : PropertyDrawer
{
    private static string[] allNames;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (allNames == null)
            allNames = Enum.GetNames(typeof(PokemonInGame));

        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
        EditorGUI.LabelField(labelRect, label);

        // Button area (right of label)
        Rect buttonRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y,
                                   position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);

        int enumIndex = property.enumValueIndex;
        string current = (enumIndex >= 0 && enumIndex < allNames.Length) ? allNames[enumIndex] : "<None>";

        // Draw dropdown button
        if (EditorGUI.DropdownButton(buttonRect, new GUIContent(current), FocusType.Keyboard))
        {
            // Show searchable popup. When a name is selected, callback sets the SerializedProperty.
            PopupWindow.Show(buttonRect, new PokemonSearchPopup(allNames, selectedIndex =>
            {
                property.enumValueIndex = selectedIndex;
                property.serializedObject.ApplyModifiedProperties();
            }));
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}


// Popup content shown when clicking the dropdown
internal class PokemonSearchPopup : PopupWindowContent
{
    private readonly string[] names;
    private readonly Action<int> onSelect;
    private string search = "";
    private Vector2 scroll;
    private List<int> filtered;

    public PokemonSearchPopup(string[] names, Action<int> onSelect)
    {
        this.names = names;
        this.onSelect = onSelect;
        UpdateFilter();
    }

    private void UpdateFilter()
    {
        if (string.IsNullOrEmpty(search))
            filtered = Enumerable.Range(0, names.Length).ToList();
        else
        {
            string s = search.ToLowerInvariant();
            filtered = Enumerable.Range(0, names.Length)
                                 .Where(i => names[i].ToLowerInvariant().Contains(s))
                                 .ToList();
        }
    }

    public override void OnGUI(Rect rect)
    {
        GUILayout.Space(4);

        EditorGUI.BeginChangeCheck();
        search = EditorGUILayout.TextField("Search", search);
        if (EditorGUI.EndChangeCheck())
            UpdateFilter();

        GUILayout.Space(4);

        // Scrollable list of matches
        scroll = EditorGUILayout.BeginScrollView(scroll);
        foreach (int i in filtered)
        {
            // Button for each filtered name
            if (GUILayout.Button(names[i], GUILayout.Height(20)))
            {
                onSelect?.Invoke(i);
                // Close the popup programmatically
                editorWindow?.Close();
            }
        }
        EditorGUILayout.EndScrollView();
    }

    public override Vector2 GetWindowSize()
    {
        return new Vector2(300, 400);
    }
}