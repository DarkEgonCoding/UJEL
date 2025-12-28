using System.Collections;
using System.Collections.Generic;
using PsLib;
using PsLib.Sim;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class TestBattleController : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI battleLogText;

    private Battle battle;
    private Parser parser;

    void Start()
    {
        //parser = new Parser();
        battle = new Battle();

        

        // battle.Start();
    }

    private void OnRawMessage(string line)
    {
        battleLogText.text += line + "\n";

        /*
        if (parser.TryParseMessage(line, out Message msg))
        {
            Debug.Log($"Parsed: {msg.GetType().Name}");
        }
        else
        {
            Debug.Log($"Unparsed: {line}");
        }
        */
    }

    public void ChooseMove1()
    {
        
    }

    public void ChooseMove2()
    {
        
    }
}
