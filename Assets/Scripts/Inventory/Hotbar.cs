using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Hotbar : MonoBehaviour
{
    public PlayerInventory inventory;
    public InventoryUI hotbarUI;
    public int itemIndex;
    public ItemData selectedItem;

    public SpriteRenderer itemIcon;

    public Sprite selectedIcon;
    public Sprite unselectedIcon;

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

    private void Start()
    {
        hotbarUI = InventoryManager.instance.hotbarInventoryUI;
        
        hotbarUI.slotObjects[itemIndex].GetComponent<InventorySlotUI>().backgroundImage.sprite = selectedIcon;
    }

    void Update()
    {
        
        // Key 1-9 → select slot
        for (int i = 0; i < 9; i++)
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                hotbarUI.slotObjects[itemIndex].GetComponent<InventorySlotUI>().backgroundImage.sprite = unselectedIcon;
                itemIndex = i;
                selectedItem = inventory.hotbarData.inventoryData.GetValueOrDefault(itemIndex).item;
                hotbarUI.slotObjects[itemIndex].GetComponent<InventorySlotUI>().backgroundImage.sprite = selectedIcon;


                UpdateSelected(inventory.hotbarData);
                onSlotChange?.Invoke();
            }

        if (Input.GetMouseButton(0) && !inventory.inventoryOpen)
        {
            TryUseSelected();
        }
        
        if (Input.GetMouseButtonUp(0))
            TweenHelper.StopEatBounce(itemIcon.transform);
        
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f && !inventory.inventoryOpen)
        {
            int oldIndex = itemIndex;

            if (scroll < 0)    // wheel up  → next slot
                itemIndex = (itemIndex + 1) % 9;
            else               // wheel down → previous slot (wrap around)
                itemIndex = (itemIndex + 8) % 9;   // +8 instead of -1 then %9

            if (itemIndex != oldIndex)
            {
                selectedItem = inventory.hotbarData.inventoryData
                    .GetValueOrDefault(itemIndex).item;
                UpdateSelected(inventory.hotbarData);
                onSlotChange?.Invoke();
            }
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
                if (used)
                {
                    TweenHelper.SwingOnce(itemIcon.transform);
                }

                break;
            
            case ItemCategory.Weapon:
                break;
            
            case ItemCategory.Consumable:
                used = selectedItem.Use(inventory.itemUser, stack);
                if (used)
                {
                    TweenHelper.StartEatBounce(itemIcon.transform);
                    inventory.hotbarData.SubtrackItemFromStack(itemIndex, InventoryManager.instance.hotbarInventoryUI);
                }
                break;
            
            case ItemCategory.Placeable:
                used = selectedItem.Use(inventory.itemUser, stack);
                if (used)
                {
                    TweenHelper.SwingOnce(itemIcon.transform);
                    inventory.hotbarData.SubtrackItemFromStack(itemIndex, InventoryManager.instance.hotbarInventoryUI);
                }

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
