using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


[System.Serializable]
public class InventoryData
{
    public Dictionary<int, ItemDataID> inventoryData = new Dictionary<int, ItemDataID>();
    
    public void SetData(int slot, string name, int amount)
    {
        inventoryData[slot] = new ItemDataID { name = name, amount = amount };
    }

}

[System.Serializable]
public struct ItemDataID
{
    public string name;
    public int amount;
}