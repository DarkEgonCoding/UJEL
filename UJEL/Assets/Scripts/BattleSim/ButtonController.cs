using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public void StartBattle()
    {
        Debug.Log("Battle Started");
    }

    public void DoMove(int move)
    {
        var player = PlayerSwitchController.instance.SelectedPlayer;

        Debug.Log($"Move chosen by player {player} with index {move}.");
    }

    public void DoSwitch(int switchIndex)
    {
        var player = PlayerSwitchController.instance.SelectedPlayer;

        Debug.Log($"Switch chosen by player {player} with index {switchIndex}.");
    }

}
