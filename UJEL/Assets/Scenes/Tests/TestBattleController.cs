
using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Concurrent;

public class TestBattleController : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI battleLogText;

    private string messageBuffer;
    private PsLib.Battle battle;
    private PsLib.Sim.Parser parser;
    private ConcurrentQueue<string> logQueue = new ConcurrentQueue<string>();

    void Start()
    {
        parser = new PsLib.Sim.Parser();
        battle = new PsLib.Battle();
        battleLogText.text = "";

        battle.Start(OnData, "{}", "{}", "gen7randombattle");
    }

    private void OnData(object sender, DataReceivedEventArgs args)
    {
        logQueue.Enqueue(args.Data);
        if (parser.TryParseMessage(args.Data, out PsLib.Sim.Messages.Message msg)) {
            UnityEngine.Debug.Log($"Parsed: {msg.stream}, {msg.group}, {msg.action.GetType().Name}");
        } else {
            UnityEngine.Debug.Log($"Unparsed: {args.Data}");
        }
    }

    void Update()
    {
        while (logQueue.TryDequeue(out string data))
        {
            UpdateLog(data);
        }
    }

    private void UpdateLog(string data)
    {
        battleLogText.text += data + "\n";
    }

    public void DoMoveP1(int move)
    {
        battle.WriteLine($">p1 move {move}");
    }

    public void DoMoveP2(int move)
    {
        battle.WriteLine($">p2 move {move}");
    }

    public void DoSwitchP1(int num)
    {
        battle.WriteLine($">p1 switch {num}");
    }

    public void DoSwitchP2(int num)
    {
        battle.WriteLine($">p2 switch {num}");

    }
}
