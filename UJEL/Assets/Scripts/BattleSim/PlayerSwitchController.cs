using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSwitchController : MonoBehaviour
{
    [Header("Assign toggles")]
    public Toggle player1Toggle;
    public Toggle player2Toggle;

    public static PlayerSwitchController instance;

    // You can use an event to notify test code
    public event Action<int> OnPlayerChanged; // 1 or 2

    private void Awake()
    {
        if (instance == null) instance = this;

        player1Toggle.onValueChanged.AddListener(isOn => { if (isOn) PlayerSelected(1); });
        player2Toggle.onValueChanged.AddListener(isOn => { if (isOn) PlayerSelected(2); });
    }

    private void Start()
    {
        player1Toggle.isOn = true;
        player2Toggle.isOn = false;   
    }

    private void PlayerSelected(int player)
    {
        Debug.Log($"Player selected: {player}");
        OnPlayerChanged?.Invoke(player);
    }

    public int SelectedPlayer => player1Toggle.isOn ? 1 : 2;

    public void SetPlayer(int player)
    {
        if (player == 1)
        {
            player1Toggle.isOn = true;
            player2Toggle.isOn = false;
        }
        else
        {
            player2Toggle.isOn = true;
            player1Toggle.isOn = false;
        }
    }

    public void SwitchPlayer()
    {
        if (player1Toggle.isOn)
        {
            player2Toggle.isOn = true;
            player1Toggle.isOn = false;
        }
        else
        {
            player2Toggle.isOn = false;
            player1Toggle.isOn = true;
        }
    }
}
