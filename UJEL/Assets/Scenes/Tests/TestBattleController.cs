using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class TestBattleController : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI battleLogText;

    private PsLib.Battle battle;
    private PsLib.Sim.Parser parser;

    void Start()
    {
        parser = new PsLib.Sim.Parser();
        battle = new PsLib.Battle();

        battle.Start(null, null, "gen7randombattle");
    }

    private void OnRawMessage(string line)
    {
        battleLogText.text += line + "\n";

        if (parser.TryParseMessage(line, out PsLib.Sim.Messages.Message msg))
        {
            Debug.Log($"Parsed: {msg.GetType().Name}");
        }
        else
        {
            Debug.Log($"Unparsed: {line}");
        }
    }

    public void ChooseMove1()
    {
        
    }

    public void ChooseMove2()
    {
        
    }
}
