using System.Collections;
using System.Collections.Generic;
using PsLib.Sim.Messages.Actions.Minor;
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
        yield return BattleSystemUI.instance.DisplayDialog(text, waitTime);
    }
}

public class PlayMoveAnimation : BattleCommand
{
    private bool isPlayerUnit;

    public PlayMoveAnimation(bool isPlayerUnit)
    {
        this.isPlayerUnit = isPlayerUnit;
    }

    public override IEnumerator Execute()
    {
        BattleSystemUI.instance.PlayAttackAnimation(isPlayerUnit);
        yield return null;
    }
}

public class PlayFaintAnimtion : BattleCommand
{
    private bool isPlayerUnit;

    public PlayFaintAnimtion(bool isPlayerUnit)
    {
        this.isPlayerUnit = isPlayerUnit;
    }

    public override IEnumerator Execute()
    {
        BattleSystemUI.instance.PlayFaintAnimation(isPlayerUnit);
        yield return null;
    }
}

public class PlayHitAnimation : BattleCommand
{
    private bool isPlayerUnit;

    public PlayHitAnimation(bool isPlayerUnit)
    {
        this.isPlayerUnit = isPlayerUnit;
    }

    public override IEnumerator Execute()
    {
        BattleSystemUI.instance.PlayHitAnimation(isPlayerUnit);
        yield return null;
    }
}

public class UpdateHP : BattleCommand
{
    private bool isPlayerUnit;
    private string newHP;
    private int hp;

    public UpdateHP(bool isPlayerUnit, string newHP)
    {
        this.isPlayerUnit = isPlayerUnit;
        this.newHP = newHP;
    }

    public UpdateHP(bool isPlayerUnit, int hp)
    {
        this.isPlayerUnit = isPlayerUnit;
        this.hp = hp;
        this.newHP = "";
    }

    public override IEnumerator Execute()
    {
        if(newHP != "")
        {
            BattleSystemUI.instance.UpdateHP(isPlayerUnit, newHP);
        }
        else
        {
            BattleSystemUI.instance.UpdateHP(isPlayerUnit, hp);
        }
        yield return null;
    }
}

public class UpdateStatus : BattleCommand
{
    private bool isPlayerUnit;
    private Status status;
    private bool cure;

    public UpdateStatus(bool isPlayerUnit, Status status, bool cure = false)
    {
        this.isPlayerUnit = isPlayerUnit;
        this.status = status;
        this.cure = cure;
    }

    public override IEnumerator Execute()
    {
        if (cure)
        {
            BattleSystemUI.instance.UpdateStatus(isPlayerUnit, status, cure);
        }
        else {
            BattleSystemUI.instance.UpdateStatus(isPlayerUnit, status);
        }
        yield return null;
    }
}
