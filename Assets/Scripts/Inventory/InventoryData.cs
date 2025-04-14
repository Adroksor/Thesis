using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class InventoryData : MonoBehaviour
{
    public Dictionary<int, ItemDataID> inventoryData = new Dictionary<int, ItemDataID>();


    public void SetData(int slot, string name, int amount)
    {
        inventoryData[slot] = new ItemDataID { name = name, amount = amount };
    }

    private void Start()
    {
        SetData(3, "Iron ore", 15);
        SetData(4, "Steel ingot", 10);
    }
}

[System.Serializable]
public struct ItemDataID
{
    public string name;
    public int amount;
}