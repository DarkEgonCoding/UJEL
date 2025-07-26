using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Cutscene Condition", menuName = "Cutscenes/Cutscene Condition")]
public class CutsceneCondition : ScriptableObject
{
    public string requiredFlag;

    public bool IsConditionMet()
    {
        return GameFlags.Instance.HasFlag(requiredFlag);
    }
}
