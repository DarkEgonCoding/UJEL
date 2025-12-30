using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleCommand
{
    /// <summary>
    /// Runs the battleCommand
    /// </summary>
    /// <returns></returns>
    public abstract IEnumerator Execute();
}

public class LogText : BattleCommand
{
    public string text;

    public LogText(string text)
    {
        this.text = text;
    }

    public override IEnumerator Execute()
    {
        UnityEngine.Debug.Log(text);
        yield return null;
    }
}
