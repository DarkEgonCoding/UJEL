using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyHandler : MonoBehaviour, ISavable
{
    private int money;

    private int currentMoneyWager;

    public int Money => money;
    public int CurrentMoneyWager => currentMoneyWager;

    public static MoneyHandler instance;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void Start()
    {
        money = 100;
    }

    public void AddMoney(int amount)
    {
        money += amount;
    }

    public void RemoveMoney(int amount)
    {
        money -= amount;
    }

    public bool CanBuy(int cost)
    {
        if (money >= cost) return true;
        else return false;
    }

    public void SetMoneyWager(int value)
    {
        currentMoneyWager = value;
    }

    public void ResetMoneyWager()
    {
        currentMoneyWager = 0;
    }

    public object CaptureState()
    {
        return money;
    }

    public void RestoreState(object state)
    {
        money = System.Convert.ToInt32(state);
    }
}
