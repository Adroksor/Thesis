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
    private void Awake()
    {
        if (selfInitialize)
        {
            InitializeInventory();
        }
        missingItem = ItemDatabaseInstance.Instance.missingItem;
        
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

    public void SaveData(InventoryData inventoryData)
    {
        inventoryData.inventoryData.Clear();
        
        foreach (InventorySlotUI slot in slots)
        {
            string itemName;
            if (slot.itemData != null)
            {
                itemName = slot.itemData.Name;
            }
            else
            {
                itemName = null;
            }
            ItemDataID item = new ItemDataID { name = itemName, amount = slot.itemCount };
            inventoryData.inventoryData.Add(slots.IndexOf(slot), item);
        }
        Debug.Log("Data saved");
    }

    public void LoadData(InventoryData inventoryData)
    {
        foreach (InventorySlotUI slot in slots)
        {
            slot.SetData(null, 0);
        }
        foreach (var (key, value) in inventoryData.inventoryData)
        {
            slots[key].SetData(ItemDatabaseInstance.Instance.GetItemByName(value.name), value.amount);
        }
        Debug.Log("Data loaded");

    }
}
