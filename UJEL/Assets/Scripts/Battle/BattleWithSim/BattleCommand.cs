using System.Collections;
using System.Collections.Generic;
using PsLib.Sim.Messages.Parts;
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

public class ActionSelection : BattleCommand
{
    private Request request;

    public ActionSelection(Request request)
    {
        this.request = request;
    }

    public override IEnumerator Execute()
    {
        BattleStateController.instance.SetCurrentRequest(request);
        // TODO: Get player feedback on move?
        yield return null;
    }
}

public class EndBattle : BattleCommand
{
    public string user;

    public EndBattle(string user)
    {
        this.user = user;
    }

    public EndBattle()
    {
        this.user = "LOSE";
    }

    public override IEnumerator Execute()
    {
        // TODO: If user is the player, they won, if it is the enemy the enemy won.
        // If user is LOSE -> default to player loses
        yield return null;
    }
}

public class WriteDialog : BattleCommand
{
    public string text;
    public float waitTime;
    
    public WriteDialog(string text, float waitTime)
    {
        this.text = text;
        this.waitTime = waitTime;
    }

    public WriteDialog(string text)
    {
        this.text = text;
        this.waitTime = 0.75f;
    }

    public override IEnumerator Execute()
    {
        yield return BattleSystemCopy.instance.DisplayDialog(text, waitTime);
    }
}

public class PlayMoveAnimation : BattleCommand
{
    public override IEnumerator Execute()
    {
        // TODO: Play the animation
        yield return null;
    }
}
