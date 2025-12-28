
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

        battle.Start(null, null, null, "gen7randombattle");
    }

    private void OnData(object sender, DataReceivedEventArgs args)
    {
        messageBuffer += args.Data;
        if (messageBuffer.Contains("\n")) {
            string[] split = messageBuffer.Split("\n");
            messageBuffer = split[1];
            if (parser.TryParseMessage(split[0], out PsLib.Sim.Messages.Message msg)) {
                UnityEngine.Debug.Log($"Parsed: {msg.GetType().Name}");
            } else {
                UnityEngine.Debug.Log($"Unparsed: {split[0]}");
            }
        }
    }

    public void ChooseMove1()
    {
        
    }

    public void ChooseMove2()
    {
        
    }
}
