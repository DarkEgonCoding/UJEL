using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Cutscene : MonoBehaviour, IPlayerTriggerable, ISavable
{
    [SerializeReference]
    [SerializeField] List<CutsceneAction> actions;

    [Header("Disable Settings")]
    [SerializeField] bool disableAfterTrigger = false;
    [SerializeField] List<CutsceneCondition> disableIfConditionsMet;
    [SerializeField] List<CutsceneCondition> enableOnlyIfConditionsMet;

    [SerializeField, HideInInspector]
    public string cutsceneId;
    public string Id => cutsceneId;
    private bool isActive = true;

    public object CaptureState()
    {
        return isActive;
    }

    public void RestoreState(object state)
    {
        isActive = (bool)state;

        if (!isActive)
        {
            DisableTrigger(); // Ensures collider and visuals match state
        }
    }

    private void OnValidate()
    {
    #if UNITY_EDITOR
        if (string.IsNullOrWhiteSpace(cutsceneId))
        {
            cutsceneId = gameObject.scene.name + "_" + gameObject.name;
            EditorUtility.SetDirty(this); // Mark scene dirty so Unity knows it changed
        }
    #endif
    }

    private void Start()
    {
        if (enableOnlyIfConditionsMet.Count > 0)
        {
            foreach (var condition in enableOnlyIfConditionsMet)
            {
                if (!condition.IsConditionMet())
                {
                    DisableTrigger();
                    return;
                }
            }
        }

        foreach (var condition in disableIfConditionsMet)
        {
            if (condition.IsConditionMet())
            {
                DisableTrigger();
                break;
            }
        }

        if (GameFlags.WasCutsceneTriggered(cutsceneId))
        {
            DisableTrigger();
            return;
        }
    }

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

        if (disableAfterTrigger)
        {
            GameFlags.MarkCutsceneTriggered(cutsceneId);
            DisableTrigger();
        }
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
        if (!isActive) return;

        player.animator.SetBool("isMoving", false);
        StartCoroutine(Play());
    }

    public void StartCutscene()
    {
        if (!isActive) return;

        PlayerController.Instance.animator.SetBool("isMoving", false);
        StartCoroutine(Play());
    }

    public void DisableTrigger()
    {
        isActive = false;
    }

    public void EnableTrigger()
    {
        isActive = true;
    }
}
