
using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class TestBattleController : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI battleLogText;

    private string messageBuffer;
    private PsLib.Battle battle;
    private PsLib.Sim.Parser parser;

    void Start()
    {
        parser = new PsLib.Sim.Parser();
        battle = new PsLib.Battle();
        battleLogText.text = "";

        battle.Start(OnData, "", "", "gen7randombattle");
    }

    private void OnData(object sender, DataReceivedEventArgs args)
    {
        if (parser.TryParseMessage(args.Data, out PsLib.Sim.Messages.Message msg)) {
            UnityEngine.Debug.Log($"Parsed: {msg.GetType().Name}");
        } else {
            UnityEngine.Debug.Log($"Unparsed: {args.Data}");
        }
    }

    public void DoMoveP1(int move)
    {
        
    }

    public void DoMoveP2(int move)
    {
        
    }

    public void DoSwitchP1(int num)
    {
        
    }

    public void DoSwitchP2(int num)
    {
        
    }

}
