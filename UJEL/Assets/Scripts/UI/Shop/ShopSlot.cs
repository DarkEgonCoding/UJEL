using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopSlot
{
    [Header("Item details")]
    [SerializeField] public ItemBase item;
    [SerializeField] public int cost;

    [Header("Item Numbers")]
    [SerializeField] public bool infiniteNumOfItem = false;
    [SerializeField][Range(0, 99)] public int itemCount;
}
