using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{

    public int inventorySize = 9;
    public GameObject slotPrefab;
    public List<InventorySlotUI> slots = new List<InventorySlotUI>();
    public ItemData missingItem;
    private void Awake()
    {
        InitializeInventory();
        missingItem = ItemDatabaseInstance.Instance.missingItem;
        
        ItemData ironOre = GetItemByName("Iron ore");
        ItemData coal = GetItemByName("Coal");
        ItemData gold = GetItemByName("Gold");
        
        ItemData item = ironOre ?? missingItem;
        SetSlotData(0, item, 20);
        SetSlotData(2, item, 20);
        
        item = coal ?? missingItem;
        SetSlotData(4, item, 20);
        SetSlotData(8, item, 20);
    }

    public void InitializeInventory()
    {
        for (int i = 0; i < inventorySize; i++)
        {
            GameObject slot = Instantiate(slotPrefab, transform);
            slots.Add(slot.GetComponent<InventorySlotUI>());
        }
    }
    
    public void SetSlotData(int slotIndex, ItemData itemData, int itemCount)
    {
        slots[slotIndex].SetData(itemData, itemCount);
    }

    public ItemData GetItemByName(string itemName)
    {
        if (ItemDatabaseInstance.Instance.TryGetItemByName(itemName, out ItemData item))
        {
            return item;
        }
        return null;
    }
}
