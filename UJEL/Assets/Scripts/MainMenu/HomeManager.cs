using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HomeManager : MonoBehaviour
{
    [SerializeField] Cutscene MomTalkCutscene;

    void Start()
    {
        if (!GameFlags.WasCutsceneTriggered(MomTalkCutscene.cutsceneId))
        {
            MomTalkCutscene.StartCutscene();
        }
    }
}
