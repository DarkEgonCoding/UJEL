using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PlasticGui;
using Codice.CM.Client.Gui;

[CustomPropertyDrawer(typeof(CutsceneActor))]
public class CutsceneActorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, label);

        var togglePos = new Rect(position.x, position.y, 70, position.height);
        var fieldPos = new Rect(position.x + 70, position.y, position.width - 70, position.height);

        var isPLayerProp = property.FindPropertyRelative("isPlayer");

        isPLayerProp.boolValue = GUI.Toggle(togglePos, isPLayerProp.boolValue, "Is Player");
        isPLayerProp.serializedObject.ApplyModifiedProperties();

        if (!isPLayerProp.boolValue)
            EditorGUI.PropertyField(fieldPos, property.FindPropertyRelative("character"), GUIContent.none);

        EditorGUI.EndProperty();
    }
}
