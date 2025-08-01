using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Cutscene))]
public class CutsceneEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var cutscene = target as Cutscene;

        base.OnInspectorGUI();

        using (var scope = new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Dialog"))
            {
                cutscene.AddAction(new DialogueAction());
            }
            else if (GUILayout.Button("Move Actor"))
            {
                cutscene.AddAction(new MoveActorAction());
            }
            else if (GUILayout.Button("Turn Actor"))
            {
                cutscene.AddAction(new TurnActorAction());
            }
        }

        using (var scope = new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Teleport Object"))
            {
                cutscene.AddAction(new TeleportObjectAction());
            }
            else if (GUILayout.Button("Enable Object"))
            {
                cutscene.AddAction(new EnableObjectAction());
            }
            if (GUILayout.Button("Disable Object"))
            {
                cutscene.AddAction(new DisableObjectAction());
            }
        }

        using (var scope = new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Fade In"))
            {
                cutscene.AddAction(new FadeInAction());
            }
            else if (GUILayout.Button("Fade Out"))
            {
                cutscene.AddAction(new FadeOutAction());
            }
            else if (GUILayout.Button("Walk To Tile"))
            {
                cutscene.AddAction(new WalkToTileAction());
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Cutscene ID", cutscene.Id, EditorStyles.helpBox);

        if (GUILayout.Button("Copy Cutscene ID to Clipboard"))
        {
            EditorGUIUtility.systemCopyBuffer = cutscene.Id;
            Debug.Log($"Copied Cutscene ID to clipboard: {cutscene.Id}");
        }
    }
}
