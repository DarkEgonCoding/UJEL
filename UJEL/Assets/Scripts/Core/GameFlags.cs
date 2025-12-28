using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameFlags : MonoBehaviour, ISavable
{
    public static GameFlags Instance { get; private set; }

    private HashSet<string> flags = new HashSet<string>();

    private static HashSet<string> triggeredCutscenes = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetFlag(string flag)
    {
        flags.Add(flag);
    }

    public bool HasFlag(string flag)
    {
        return flags.Contains(flag);
    }

    public static void MarkCutsceneTriggered(string id)
    {
        triggeredCutscenes.Add(id);
    }

    public static bool WasCutsceneTriggered(string id)
    {
        return triggeredCutscenes.Contains(id);
    }
    
    [System.Serializable]
    private class GameFlagsSaveData
    {
        public List<string> flags;
        public List<string> cutscenes;
    }

    public object CaptureState()
    {
        return new GameFlagsSaveData
        {
            flags = new List<string>(flags),
            cutscenes = new List<string>(triggeredCutscenes)
        };
    }

    public void RestoreState(object state)
    {
        var saveData = state as GameFlagsSaveData;
        if (saveData == null) return;

        flags = new HashSet<string>(saveData.flags ?? new List<string>());
        triggeredCutscenes = new HashSet<string>(saveData.cutscenes ?? new List<string>());

        //Debug.Log("Restored Data for gameflags");
        //DebugFlags();
    }

    private void DebugFlags()
    {
        if (flags == null || flags.Count == 0) Debug.Log("Flags is null or empty.");
        foreach (string flag in flags)
        {
            Debug.Log(flag);
        }

        if (triggeredCutscenes == null || triggeredCutscenes.Count == 0) Debug.Log("Triggered cutscenes is null or empty.");
        foreach (string cutscene in triggeredCutscenes)
        {
            Debug.Log("Cutscene: " + cutscene);
        }
    }
}
