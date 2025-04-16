using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


[System.Serializable]
public class InventoryData
{
    public Dictionary<int, ItemDataID> inventoryData = new Dictionary<int, ItemDataID>();
    public int inventorySize;

    public InventoryData(int size)
    {
        inventorySize = size;
        for (int i = 0; i < size; i++)
        {
            inventoryData[i] = new ItemDataID { name = null, amount = 0 };
        }
    }
    
    public void SetData(int slot, string name, int amount)
    {
        if (name == null || amount == 0)
        {
            return;
        }
        inventoryData[slot] = new ItemDataID { name = name, amount = amount };
    }

}

[System.Serializable]
public struct ItemDataID
{
    public string name;
    public int amount;
}