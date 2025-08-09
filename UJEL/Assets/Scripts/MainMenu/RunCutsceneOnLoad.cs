using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RunCutsceneOnLoad : MonoBehaviour
{
    [SerializeField] Cutscene CutsceneToRun;

    void Start()
    {
        if (!GameFlags.WasCutsceneTriggered(CutsceneToRun.cutsceneId))
        {
            CutsceneToRun.StartCutscene();
        }
    }
}
