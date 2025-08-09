using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public void StartBattle()
    {
        Debug.Log("Battle Started");
        MainBattleSimController.instance.BattleStart();
    }

    public void DoMoveBtn(int move)
    {
        var player = PlayerSwitchController.instance.SelectedPlayer;
        MainBattleSimController.instance.DoMove(move, player);
    }

    public void DoSwitchBtn(int switchIndex)
    {
        var player = PlayerSwitchController.instance.SelectedPlayer;
        MainBattleSimController.instance.DoSwitch(switchIndex, player);
    }

}
