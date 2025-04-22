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
        missingItem = ItemDatabaseInstance.instance.missingItem;
        
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


}
