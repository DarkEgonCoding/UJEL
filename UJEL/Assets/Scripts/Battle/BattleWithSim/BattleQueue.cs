using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Concurrent;

public class BattleQueue : MonoBehaviour
{
    private PsLib.Battle battle;
    private PsLib.Sim.Parser parser;
    private bool isExecutingCommand = false;
    private ConcurrentQueue<BattleCommand> commandQueue = new ConcurrentQueue<BattleCommand>();

    /// <summary>
    /// Sets up the battle and starts the server
    /// </summary>
    public void Init(string packedTeamOne, string packedTeamTwo)
    {
        parser = new PsLib.Sim.Parser();
        battle = new PsLib.Battle();

        // TODO: Change gen7randombattle to gen9custombattle or something similar
        battle.Start(OnData, "{}", "{}", "gen7randombattle");
    }

    /// <summary>
    /// Checks the commandQueue to run commands
    /// </summary>
    void Update()
    {
        if (isExecutingCommand) return;

        if (commandQueue.TryDequeue(out var cmd))
            StartCoroutine(ExecuteCommand(cmd));
    }

    /// <summary>
    /// Calls the command's execute function
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    IEnumerator ExecuteCommand(BattleCommand cmd)
    {
        isExecutingCommand = true;
        yield return cmd.Execute();
        isExecutingCommand = false;
    }

    /// <summary>
    /// Runs on receiving data from the server.
    /// Calls the message translator to convert the messages to BattleCommands
    /// which will be run by the battle system.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void OnData(object sender, DataReceivedEventArgs args)
    {
        if (parser.TryParseMessage(args.Data, out PsLib.Sim.Messages.Message msg)) {
            UnityEngine.Debug.Log($"Parsed: {msg.stream}, {msg.group}, {msg.action.GetType().Name}");
            
            // Add every command to the ConcurrentQueue
            var commands = msg.action.GetCommands(msg.stream);
            foreach (var cmd in commands)
            {
                commandQueue.Enqueue(cmd);
            }
        } else {
            UnityEngine.Debug.Log($"Unparsed: {args.Data}");
        }
    }

    /// <summary>
    /// Calls the server to run a player's move choice
    /// </summary>
    /// <param name="player"></param>
    /// <param name="move"></param>
    public void DoMove(int player, int move)
    {
        if (player >= 1 && player <= 2)
            battle.WriteLine($">p{player} move {move}");
        else
            UnityEngine.Debug.LogWarning($"Player input: {player} is invalid");
    }

    /// <summary>
    /// Calls the server to run a player's switch choice
    /// </summary>
    /// <param name="player"></param>
    /// <param name="num"></param>
    public void DoSwitch(int player, int num)
    {
        if (player >= 1 && player <= 2)
            battle.WriteLine($">p{player} switch {num}");
    }
}
