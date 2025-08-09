using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorldDialogObject))]
public class WorldDialogEditor : Editor
{
    SerializedProperty requireDirectionProp;
    SerializedProperty requiredDirectionProp;

    void OnEnable()
    {
        // Link to the serialized properties
        requireDirectionProp = serializedObject.FindProperty("RequireDirection");
        requiredDirectionProp = serializedObject.FindProperty("requiredDirection");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, "RequireDirection", "requiredDirection", "m_Script");

        EditorGUILayout.PropertyField(requireDirectionProp);

        if (requireDirectionProp.boolValue)
        {
            EditorGUILayout.PropertyField(requiredDirectionProp);
        }

        if (serializedObject.ApplyModifiedProperties())
        {
            EditorUtility.SetDirty(target);
        }
    }
}
