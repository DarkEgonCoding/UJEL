using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Cutscene : MonoBehaviour, IPlayerTriggerable
{
    [SerializeReference]
    [SerializeField] List<CutsceneAction> actions;

    [Header("Disable Settings")]
    [SerializeField] bool disableAfterTrigger = false;
    [SerializeField] List<CutsceneCondition> disableIfConditionsMet;
    [SerializeField] List<CutsceneCondition> enableOnlyIfConditionsMet;
    private bool isActive = true;

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
            DisableTrigger();
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
        
        var collider = GetComponent<Collider2D>();
        if (collider) collider.enabled = false;
    }

    public void EnableTrigger()
    {
        isActive = true;
        var collider = GetComponent<Collider2D>();
        if (collider) collider.enabled = true;
    }
}
