using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Hotbar : MonoBehaviour
{
    public PlayerInventory inventory;

    public int itemIndex;
    public ItemData selectedItem;

    public SpriteRenderer itemIcon;

    public Action onSlotChange;

    private void OnEnable()
    {
        InventoryManager.instance.OnInventoryChanged += UpdateSelected;
        onSlotChange += UpdateItemIcon;
    }

    private void OnDisable()
    {       
        InventoryManager.instance.OnInventoryChanged -= UpdateSelected;
        onSlotChange -= UpdateItemIcon;

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
                onSlotChange?.Invoke();
            }

        if (Input.GetMouseButton(0) && !inventory.inventoryOpen)
        {
            TryUseSelected();
        }
    }

    public void UpdateItemIcon()
    {
        if (selectedItem == null)
        {
            itemIcon.sprite = null;
            return;
        }
        itemIcon.sprite = selectedItem.ItemImage;
    }
    
    void TryUseSelected()
    {
        ItemStack stack = inventory.hotbarData.inventoryData.GetValueOrDefault(itemIndex);
        if (selectedItem == null || inventory.inventoryOpen)
            return;
        bool used;
        switch (selectedItem.Itemtype)
        {
            case ItemCategory.Tool:
                used = selectedItem.Use(inventory.itemUser, stack);
                if(used)
                    TweenHelper.SwingOnce(itemIcon.transform);

                break;
            case ItemCategory.Weapon:
            case ItemCategory.Consumable:
                used = selectedItem.Use(inventory.itemUser, stack);
                TweenHelper.SwingOnce(itemIcon.transform);
                if(used)
                    inventory.hotbarData.SubtrackItemFromStack(itemIndex, InventoryManager.instance.hotbarInventoryUI);
                break;
            case ItemCategory.Placeable:
                used = selectedItem.Use(inventory.itemUser, stack);
                TweenHelper.SwingOnce(itemIcon.transform);
                if(used)
                    inventory.hotbarData.SubtrackItemFromStack(itemIndex, InventoryManager.instance.hotbarInventoryUI);

                break;
            default:
                Debug.Log($"Item {selectedItem.name} not usable from hot-bar");
                break;
        }
        UpdateSelected(inventory.hotbarData);
        UpdateItemIcon();
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
