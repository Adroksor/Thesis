using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class InventoryData
{
    public Dictionary<int, ItemStack> inventoryData = new Dictionary<int, ItemStack>();
    public int inventorySize;

    public InventoryData(int size)
    {
        inventorySize = size;
        for (int i = 0; i < size; i++)
        {
            inventoryData[i] = new ItemStack { item = null, amount = 0 };
        }
    }

    public void SubtrackItemFromStack(int slot, InventoryUI UI)
    {
        ItemStack item = GetItemAtIndex(slot);
        
        if (item.amount - 1 == 0)
        {
            ItemStack itemStack = new ItemStack{ item = null, amount = 0};
            inventoryData[slot] = itemStack; 
        }
        else
        {
            ItemStack itemStack = new ItemStack{ item = item.item, amount = item.amount - 1};
            inventoryData[slot] = itemStack; 
        }
        InventoryManager.instance.LoadData(this, UI);
    }
    
    public void SetData(int slot, ItemStack itemStack)
    {
        if (string.IsNullOrEmpty(itemStack.item.name) || itemStack.amount == 0)
        {
            return;
        }
        inventoryData[slot] = new ItemStack { item = itemStack.item, amount = itemStack.amount };
    }

    public ItemStack GetItemAtIndex(int slot)
    {
        return inventoryData[slot];
    }
    public int GetIndexOfItem(ItemData target)
    {
        foreach (var kvp in inventoryData)
        {
            if (kvp.Value.item == target && kvp.Value.amount != 0)
                return kvp.Key;
        }
        return -1;
    }

    
    public List<SlotSaveData> ToSaveList()
    {
        List<SlotSaveData> list = new();

        foreach (var kvp in inventoryData)
        {
            if (kvp.Value.item == null || kvp.Value.amount == 0)
                continue;                       // skip empty slots (optional)

            list.Add(new SlotSaveData
            {
                slotIndex = kvp.Key,
                itemName  = kvp.Value.item.name,
                amount    = kvp.Value.amount
            });
        }
        return list;
    }

    public void FromSaveList(List<SlotSaveData> list)
    {
        // clear but keep size
        foreach (int key in new List<int>(inventoryData.Keys))
            inventoryData[key] = new ItemStack();

        foreach (var e in list)
        {
            inventoryData[e.slotIndex] = new ItemStack
            {
                item   = ItemDatabaseInstance.instance.GetItemByname(e.itemName),
                amount = e.amount
            };
        }
    }
}

[System.Serializable]
public struct ItemStack
{
    public ItemData item;
    public int amount;
}