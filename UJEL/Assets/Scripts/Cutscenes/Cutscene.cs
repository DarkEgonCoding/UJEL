using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Cutscene : MonoBehaviour, IPlayerTriggerable
{
    [SerializeReference]
    [SerializeField] List<CutsceneAction> actions;

    public IEnumerator Play()
    {
        GameController.instance.StartCutsceneState();

        foreach (var action in actions)
        {
            if (action.WaitForCompletion) yield return action.Play();
            else
            {
                StartCoroutine(action.Play());
            }
        }

        GameController.instance.StartFreeRoamState();
    }

    public void AddAction(CutsceneAction action)
    {
#if UNITY_EDITOR
        Undo.RegisterCompleteObjectUndo(this, "Add action to cutscene.");
#endif

        action.Name = action.GetType().ToString();
        actions.Add(action);
    }

    public void OnPlayerTriggered(PlayerController player)
    {
        player.animator.SetBool("isMoving", false);
        StartCoroutine(Play());
    }
}
