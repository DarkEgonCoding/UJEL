using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopSlot : ISavable
{
    [Header("Item details")]
    [SerializeField] public ItemBase item;
    [SerializeField] public int cost;

    [Header("Item Numbers")]
    [SerializeField] public bool infiniteNumOfItem = false;
    [SerializeField][Range(0, 99)] public int itemCount;

    public object CaptureState()
    {
        return new ShopSlotSavableData
        {
            itemCount = this.itemCount
        };
    }

    public void RestoreState(object state)
    {
        var data = (ShopSlotSavableData)state;
        this.itemCount = data.itemCount;
    }
}

[System.Serializable]
public class ShopSlotSavableData
{
    public int itemCount;
}
