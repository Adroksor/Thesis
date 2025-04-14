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
        
        ItemData ironOre = GetItemByName("Iron ore");
        ItemData coal = GetItemByName("Coal");
        ItemData gold = GetItemByName("Gold");
        
        ItemData item = coal ?? missingItem;
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

    public void SaveData(InventoryData inventoryData)
    {
        foreach (InventorySlotUI slot in slots)
        {
            ItemDataID item = new ItemDataID { name = slot.itemData.Name, amount = slot.itemCount };
            inventoryData.inventoryData.Add(slots.IndexOf(slot), item);
        }
    }

    public void LoadData(InventoryData inventoryData)
    {
        foreach (var (key, value) in inventoryData.inventoryData)
        {
            slots[key].SetData(ItemDatabaseInstance.Instance.GetItemByName(value.name), value.amount);
        }
    }
}
