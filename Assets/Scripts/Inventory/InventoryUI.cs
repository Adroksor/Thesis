using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public bool selfInitialize = true;
    public int inventorySize = 9;
    public GameObject slotPrefab;
    public List<InventorySlotUI> slots = new List<InventorySlotUI>();
    public ItemData missingItem;
    InventoryData inventoryData;
    private void Awake()
    {
        if (selfInitialize)
        {
            InitializeInventory();
        }
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

    private void OnEnable()
    {
        InventoryManager.instance.SubscribeSlotsToEvents(slots);
    }

    private void OnDisable()
    {
        InventoryManager.instance.UnnsubscribeSlotsToEvents(slots);

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

    public void SaveData()
    {
        foreach (InventorySlotUI slot in slots)
        {
            inventoryData.inventoryData.Add(slot.itemData,slot.itemCount);
        }
    }

    public void LoadData()
    {
        int index = 0;
        foreach (var (key, value) in inventoryData.inventoryData)
        {
            slots[index].itemData = key;
            slots[index].itemCount = value;
            index++;

        }
    }
}
