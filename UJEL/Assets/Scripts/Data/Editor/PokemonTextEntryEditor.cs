using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(PokemonTextEntry))]
public class PokemonTextEntryDrawer : PropertyDrawer
{
    private string[] allPokemonNames;
    private string[] allMoveNames;

    private void Init()
    {
        if (allPokemonNames == null)
        {
            allPokemonNames = Enum.GetNames(typeof(PokemonInGame))
                .ToArray();

            allMoveNames = PokemonTextEntryExtensions.ALL_MOVES
                .Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries)
                .ToArray();
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Init();
        EditorGUI.BeginProperty(position, label, property);

        // Draw box background for clarity
        GUI.Box(position, GUIContent.none);

        // Find properties
        var pokemonEnumProp = property.FindPropertyRelative("pokemonEnum");
        var levelProp = property.FindPropertyRelative("level");
        var movesProp = property.FindPropertyRelative("moves");

        float lineHeight = EditorGUIUtility.singleLineHeight + 2;
        Rect row = new Rect(position.x + 4, position.y + 2, position.width - 8, EditorGUIUtility.singleLineHeight);

        // --- Pokemon searchable dropdown ---
        int currentPokemonIndex = pokemonEnumProp.enumValueIndex;
        string currentPokemonName = allPokemonNames[Mathf.Clamp(currentPokemonIndex, 0, allPokemonNames.Length - 1)];
        if (EditorGUI.DropdownButton(row, new GUIContent(currentPokemonName), FocusType.Keyboard))
        {
            SearchablePopup.Show(row, allPokemonNames, currentPokemonIndex, i =>
            {
                pokemonEnumProp.enumValueIndex = i;
                property.serializedObject.ApplyModifiedProperties();
            });
        }

        // --- Level ---
        row.y += lineHeight;
        EditorGUI.PropertyField(row, levelProp);

        // --- Moves list ---
        row.y += lineHeight;
        EditorGUI.LabelField(row, "Moves");

        for (int i = 0; i < movesProp.arraySize; i++)
        {
            row.y += lineHeight;
            var moveProp = movesProp.GetArrayElementAtIndex(i);
            int selectedIndex = Array.IndexOf(allMoveNames, moveProp.stringValue);
            if (selectedIndex < 0) selectedIndex = 0;
            string currentMoveName = allMoveNames[selectedIndex];

            if (EditorGUI.DropdownButton(row, new GUIContent(currentMoveName), FocusType.Keyboard))
            {
                SearchablePopup.Show(row, allMoveNames, selectedIndex, newIndex =>
                {
                    moveProp.stringValue = allMoveNames[newIndex];
                    property.serializedObject.ApplyModifiedProperties();
                });
            }
        }

        // Add/remove buttons
        row.y += lineHeight;
        bool canAdd = movesProp.arraySize < 4;
        EditorGUI.BeginDisabledGroup(!canAdd);
        if (GUI.Button(new Rect(row.x, row.y, 80, EditorGUIUtility.singleLineHeight), "Add Move"))
        {
            movesProp.InsertArrayElementAtIndex(movesProp.arraySize);
            movesProp.GetArrayElementAtIndex(movesProp.arraySize - 1).stringValue = allMoveNames[0]; // default
        }
        EditorGUI.EndDisabledGroup();

        if (GUI.Button(new Rect(row.x + 85, row.y, 110, EditorGUIUtility.singleLineHeight), "Remove Last") && movesProp.arraySize > 0)
        {
            movesProp.DeleteArrayElementAtIndex(movesProp.arraySize - 1);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var movesProp = property.FindPropertyRelative("moves");
        return (3 + movesProp.arraySize + 1) * (EditorGUIUtility.singleLineHeight + 2) + 4;
    }
}

public class SearchablePopup : PopupWindowContent
{
    private readonly string[] items;
    private readonly Action<int> onSelect;
    private readonly int startIndex;

    private string search = "";
    private Vector2 scroll;

    /// <summary>
    /// Show the popup.
    /// </summary>
    /// <param name="activatorRect">Rect to anchor to (pass the button rect)</param>
    /// <param name="items">Array of option strings</param>
    /// <param name="currentIndex">index currently selected</param>
    /// <param name="onSelect">callback when an item is chosen</param>
    public static void Show(Rect activatorRect, string[] items, int currentIndex, Action<int> onSelect)
    {
        var popup = new SearchablePopup(items ?? new string[0], currentIndex, onSelect);
        PopupWindow.Show(activatorRect, popup);
    }

    private SearchablePopup(string[] items, int startIndex, Action<int> onSelect)
    {
        this.items = items;
        this.onSelect = onSelect;
        this.startIndex = Mathf.Clamp(startIndex, 0, Math.Max(0, items.Length - 1));
    }

    public override Vector2 GetWindowSize()
    {
        // width x height
        return new Vector2(300, 360);
    }

    public override void OnOpen()
    {
        base.OnOpen();
        // give the search box focus when opened
        EditorApplication.delayCall += () =>
        {
            // ensure the window is still open
            try { EditorGUI.FocusTextInControl("SearchField"); } catch { }
        };
    }

    public override void OnGUI(Rect rect)
    {
        GUILayout.BeginVertical();
        GUILayout.Space(4);

        // Search field
        GUI.SetNextControlName("SearchField");
        EditorGUI.BeginChangeCheck();
        search = EditorGUILayout.TextField("Search", search);
        if (EditorGUI.EndChangeCheck())
        {
            // reset scroll to top when search changes
            scroll = Vector2.zero;
        }

        GUILayout.Space(4);

        // Build filtered indices once per frame
        string s = search?.Trim().ToLowerInvariant();
        IEnumerable<int> filtered = Enumerable.Range(0, items.Length);
        if (!string.IsNullOrEmpty(s))
            filtered = filtered.Where(i => items[i].ToLowerInvariant().Contains(s));

        // Scroll list
        scroll = EditorGUILayout.BeginScrollView(scroll);
        bool any = false;
        foreach (int i in filtered)
        {
            any = true;
            // highlight currently selected
            GUIStyle style = (i == startIndex) ? EditorStyles.boldLabel : EditorStyles.label;
            if (GUILayout.Button(items[i], style, GUILayout.Height(20)))
            {
                onSelect?.Invoke(i);
                editorWindow?.Close();
                return;
            }
        }

        if (!any)
        {
            GUILayout.Label("No results", EditorStyles.centeredGreyMiniLabel);
        }

        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
    }
}