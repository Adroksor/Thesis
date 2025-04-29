using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotbar : MonoBehaviour
{
    public PlayerInventory inventory;

    public int itemIndex;
    public ItemStack itemStack;
    public ItemData selectedItem;


    private void OnEnable()
    {
        InventoryManager.instance.OnInventoryChanged += UpdateSelected;
    }

    private void OnDisable()
    {       
        InventoryManager.instance.OnInventoryChanged -= UpdateSelected;
    }

    void Update()
    {
        // Key 1-9 → select slot
        for (int i = 0; i < 9; i++)
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                itemIndex = i;
                selectedItem = inventory.hotbarData.inventoryData.GetValueOrDefault(itemIndex).item;
                UpdateSelected(inventory.hotbarData);
            }

        if (Input.GetMouseButtonDown(0))
        {
            TryUseSelected();
        }
    }
    
    void TryUseSelected()
    {
        ItemStack stack = inventory.hotbarData.inventoryData.GetValueOrDefault(itemIndex);
        if (selectedItem == null || inventory.inventoryOpen)
            return;
        switch (selectedItem.Itemtype)
        {
            case ItemCategory.Tool:
            case ItemCategory.Weapon:
            case ItemCategory.Consumable:
                selectedItem.Use(inventory.itemUser, stack);
                break;
            case ItemCategory.Placeable:
                selectedItem.Use(inventory.itemUser, stack);
                
                break;
            default:
                Debug.Log($"Item {selectedItem.name} not usable from hot-bar");
                break;
        }
        UpdateSelected(inventory.hotbarData);
    }

    public void UpdateSelected(InventoryData inventoryData)
    {
        ItemData data = inventoryData.inventoryData.GetValueOrDefault(itemIndex).item;
        selectedItem = data;
        if (data is BuildableItem buildable)
        {
            BuildingPlacer.instance.selectedBuilding = buildable.buildingPrefab;
            BuildingPlacer.instance.UpdateGhostObject();
        }
        else
        {
            BuildingPlacer.instance.selectedBuilding = null;
            BuildingPlacer.instance.DestroyGhostBuilding();
        }
    }
}
