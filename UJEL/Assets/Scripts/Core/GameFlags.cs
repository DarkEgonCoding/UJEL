using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlags : MonoBehaviour
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
}
