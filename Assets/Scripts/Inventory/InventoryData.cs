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

    public ItemDataID GetItemAtIndex(int slot)
    {
        return inventoryData[slot];
    }
    
    public List<SlotSaveData> ToSaveList()
    {
        List<SlotSaveData> list = new();

        foreach (var kvp in inventoryData)
        {
            if (string.IsNullOrEmpty(kvp.Value.name) || kvp.Value.amount == 0)
                continue;                       // skip empty slots (optional)

            list.Add(new SlotSaveData
            {
                slotIndex = kvp.Key,
                itemName  = kvp.Value.name,
                amount    = kvp.Value.amount
            });
        }
        return list;
    }

    public void FromSaveList(List<SlotSaveData> list)
    {
        // clear but keep size
        foreach (int key in new List<int>(inventoryData.Keys))
            inventoryData[key] = new ItemDataID();

        foreach (var e in list)
        {
            inventoryData[e.slotIndex] = new ItemDataID
            {
                name   = e.itemName,
                amount = e.amount
            };
        }
    }

    
}

[System.Serializable]
public struct ItemDataID
{
    public string name;
    public int amount;
}