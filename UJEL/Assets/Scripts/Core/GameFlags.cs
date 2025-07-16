using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameFlags : MonoBehaviour, ISavable
{
    public static GameFlags Instance { get; private set; }

    private HashSet<string> flags = new HashSet<string>();

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

    public object CaptureState()
    {
        return new List<string>(flags);
    }

    public void RestoreState(object state)
    {
        flags = new HashSet<string>((List<string>)state);
    }

    private static HashSet<string> triggeredCutscenes = new HashSet<string>();

    public static void MarkCutsceneTriggered(string id)
    {
        triggeredCutscenes.Add(id);
    }

    public static bool WasCutsceneTriggered(string id)
    {
        return triggeredCutscenes.Contains(id);
    }

    // Optional: Save/Load support
    public static object CaptureCutsceneFlags()
    {
        return triggeredCutscenes.ToList();
    }

    public static void RestoreCutsceneFlags(object state)
    {
        triggeredCutscenes = new HashSet<string>((List<string>)state);
    }
}
