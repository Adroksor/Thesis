using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class InventoryManager : MonoBehaviour
{   
    public static InventoryManager instance;
    
    public PlayerInventory playerInventory;
    public InventoryUI playerInventoryUI;
    public InventoryUI hotbarInventoryUI;
    public InventoryUI equipmentInventory;

    public List<InventoryUI> inventoriyUIs;
    
    [Header("UI Elements")]
    public GameObject FurnaceUI;
    public GameObject WorkbenchUI;
    [Header("Buildings Recipes")]


    public PlayerInventory playerInventoryScript;
    
    public InventorySlotUI initialSlot;
    public ItemData draggedItem;
    public int draggedItemCount;
    
    [Header("OpenInventory")]
    public InventoryUI currentlyOpenedInventory;
    public GameObject currentlyInteractedObject;

    public GameObject droppedItem;

    
    public Action OnInventoryChanged;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    

    public void SubscribeSlotsToEvents(List<InventorySlotUI> inventory)
    {
        foreach (InventorySlotUI slot in inventory)
        {
            slot.onBeginDragLeft += OnBeginDragLeft;
            slot.onBeginDragRight += OnBeginDragRight;
            slot.onEndDrag += OnEndDrag;
            slot.onDropLeft += OnDropLeft;
            slot.onDropRight += OnDropRight;
            slot.onClick += OnClick;
        }
    }

    public void UnnsubscribeSlotsToEvents(List<InventorySlotUI> inventory)
    {
        foreach (InventorySlotUI slot in inventory)
        {
            slot.onBeginDragLeft -= OnBeginDragLeft;
            slot.onBeginDragRight -= OnBeginDragRight;
            slot.onEndDrag -= OnEndDrag;
            slot.onDropLeft -= OnDropLeft;
            slot.onDropRight -= OnDropRight;
            slot.onClick -= OnClick;
        }
    }
    

    private void OnBeginDragRight(InventorySlotUI slot, InventoryUI inventory)
    {
        initialSlot = slot;
        draggedItem = slot.itemData;
        draggedItemCount = slot.itemCount;
    }

    private void OnBeginDragLeft(InventorySlotUI slot, InventoryUI inventory)
    {
        initialSlot = slot;
        draggedItem = slot.itemData;
        draggedItemCount = slot.itemCount;
        
    }

    private void OnEndDrag(InventorySlotUI slot, InventoryUI inventory)
    {
        
    }

    private void OnDropLeft(InventorySlotUI slot, InventoryUI inventory)
    {
        if (initialSlot == slot)
        {
            return;
        }
        if (draggedItem != null)
        {
            if (slot.itemData == null)
            {
                initialSlot.SetData(null, 0);
                slot.SetData(draggedItem, draggedItemCount);
            }
            else if (slot.itemData == draggedItem)
            {
                int leftover, stackSize = draggedItem.MaxStackSize;
                leftover = (draggedItemCount + slot.itemCount) - stackSize;
                if (leftover > 0)
                {
                    slot.SetData(draggedItem, stackSize);
                }
                else
                {
                    slot.SetData(draggedItem, draggedItemCount + slot.itemCount);
                }
                initialSlot.SetData(draggedItem, leftover);
                
            }
            else if (slot.itemData != draggedItem)
            {
                initialSlot.SetData(slot.itemData, slot.itemCount);
                slot.SetData(draggedItem, draggedItemCount);
            }
        }
        SaveEQandHotbar();
    }
    
    private void OnDropRight(InventorySlotUI slot, InventoryUI inventory)
    {
        if (initialSlot == slot)
        {
            return;
        }
        if (draggedItem != null)
        {
            if (slot.itemData == null)
            {
                int firstHalf, secondHalf;
                firstHalf = draggedItemCount / 2;
                secondHalf = draggedItemCount - firstHalf;
                if (firstHalf > 0)
                {
                    initialSlot.SetData(draggedItem, firstHalf);
                    slot.SetData(draggedItem, secondHalf + slot.itemCount);
                }
                else
                {
                    initialSlot.SetData(null, 0);
                }
                slot.SetData(draggedItem, secondHalf);
            }
            else if (slot.itemData == draggedItem)
            {
                int leftover, stackSize = draggedItem.MaxStackSize;
                int firstHalf, secondHalf;
                firstHalf = draggedItemCount / 2;
                secondHalf = draggedItemCount - firstHalf;
                leftover = (firstHalf + slot.itemCount) - stackSize;
                if (leftover > 0)
                {
                    slot.SetData(draggedItem, stackSize);
                    initialSlot.SetData(draggedItem, leftover);

                }
                else
                {
                    slot.SetData(draggedItem, secondHalf + slot.itemCount);
                    initialSlot.SetData(draggedItem, firstHalf);

                }

            }
        }
        SaveEQandHotbar();
    }
    private void OnClick(InventorySlotUI slot, InventoryUI inventory)
    {
        ItemData clickedItem = slot.itemData;
        int clickedItemCount = slot.itemCount;

        if (clickedItem == null || clickedItemCount <= 0)
            return;
        
        if (Input.GetKey(KeyCode.LeftShift))
        {
            InventoryUI destinationInventory;

            if (inventory == hotbarInventoryUI && !playerInventory.inventoryOpen)
            {
                return;
            }
            
            if (currentlyOpenedInventory == null)
                destinationInventory = hotbarInventoryUI;
            else
                destinationInventory = currentlyOpenedInventory.GetComponentInChildren<InventoryUI>();

            if (destinationInventory == inventory)
                destinationInventory = playerInventoryUI;

            

            int leftover = TryAddItemToInventory(clickedItem, clickedItemCount, destinationInventory);

            if (leftover == 0)
                slot.SetData(null, 0);
            else
                slot.SetData(clickedItem, leftover);
        }
        else
        {
            if (clickedItem.Itemtype == ItemCategory.Placeable)
            {
                BuildingPlacer buildingPlacer = GameManager.instance.buildingPlacer;
                GameObject building = GameManager.instance.GetObjectByName(clickedItem.name);
                if (building != null)
                {
                    buildingPlacer.UpdateGhostObject(building);
                    buildingPlacer.UpdateGhostPosition();
                    playerInventory.CloseInventory();
                }
            }
        }
    }

    public void SaveEQandHotbar()
    {
        SaveData(playerInventoryScript.inventoryData, playerInventoryUI);
        SaveData(playerInventoryScript.hotbarData, hotbarInventoryUI);
    }
    
    public void SaveData(InventoryData inventoryData, InventoryUI inventoryUI)
    {
        inventoryData.inventoryData.Clear();

        for (int i = 0; i < inventoryUI.slots.Count; i++)
        {
            var slot = inventoryUI.slots[i];

            if (slot.itemData != null)
            {
                inventoryData.inventoryData[i] = new ItemStack { item = slot.itemData, amount = slot.itemCount };
            }
            else
            {
                // keep an explicit “empty” entry
                inventoryData.inventoryData[i] = new ItemStack { item = null, amount = 0 };
            }
        }
        OnInventoryChanged?.Invoke();
    }
    
    public void LoadData(InventoryData inventoryData, InventoryUI inventoryUI)
    {
        if (inventoryUI.slots.Count == 0)
        {
            Debug.Log("No slots available");
        }
        foreach (InventorySlotUI slot in inventoryUI.slots)
        {
            slot.SetData(null, 0);
        }
        foreach (var (key, value) in inventoryData.inventoryData)
        {
            inventoryUI.slots[key].SetData(value.item, value.amount);
        }
    }

    
    public int TryAddItemToInventory(ItemData itemData, int amount, InventoryUI targetInventory)
    {
        if (itemData == null || amount <= 0 || targetInventory == null)
            return amount;

        int index = IndexOfFirstOrEmptySlot(targetInventory, 0, itemData.name);

        while (amount > 0 && index != -1)
        {
            InventorySlotUI slot = targetInventory.slots[index];

            int currentCount = slot.itemCount;
            int space = itemData.MaxStackSize - currentCount;

            if (slot.itemData == null || slot.itemData == itemData)
            {
                int addAmount = Mathf.Min(space, amount);
                slot.SetData(itemData, currentCount + addAmount);
                amount -= addAmount;

                if (amount > 0)
                    index = IndexOfFirstOrEmptySlot(targetInventory, index + 1, itemData.name);
            }
            else
            {
                index = IndexOfFirstOrEmptySlot(targetInventory, index + 1, itemData.name);
            }
        }
        return amount; // Return leftover amount
    }


    public int IndexOfFirstOrEmptySlot(InventoryUI inventoryUI, int startIndex = 0, string itemToTransfer = null)
    {
        for (int i = startIndex; i < inventoryUI.slots.Count; i++)
        {
            if (inventoryUI.slots[i].itemData ==null || inventoryUI.slots[i].itemData.name == itemToTransfer)
            {
                return i;
            }
        }
        return -1;
    }

    public int TryAddItemToInventoryData(ItemData itemData, int amount, InventoryData targetInventoryData)
    {
        int index = -1;
        while (amount > 0)
        {
            index = IndexOfFirstOrEmptySlotData(targetInventoryData, index + 1, itemData);
            if (index == -1) return amount;
            int maxStackSize = itemData.MaxStackSize;
            targetInventoryData.inventoryData.TryGetValue(index, out ItemStack itemAtIndex);

            if (itemAtIndex.item == null)
            {
                int added = Mathf.Min(amount, maxStackSize);
                targetInventoryData.inventoryData[index] = new ItemStack { item = itemData, amount = added };
                amount -= added;
            }
            else if (itemAtIndex.item != null && itemAtIndex.amount < maxStackSize)
            {
                int spaceLeft = maxStackSize - itemAtIndex.amount;
                int added = Mathf.Min(amount, spaceLeft);
                targetInventoryData.inventoryData[index] = new ItemStack { item = itemData, amount = itemAtIndex.amount + added };
                amount -= added;
            }
        }

        if (playerInventoryUI.isActiveAndEnabled)
        {
            LoadData(targetInventoryData, playerInventoryUI);
        }
        return amount;
    }
    
    public int TryAddItemsToPlayerData(ItemData itemData, int amount)
    {
        if (itemData == null || amount <= 0) return amount;

        int remaining = amount;

        remaining = TryAddItemToInventoryData(itemData,
            remaining,
            playerInventoryScript.hotbarData);

        if (remaining > 0)
        {
            remaining = TryAddItemToInventoryData(itemData,
                remaining,
                playerInventoryScript.inventoryData);
        }

        if (hotbarInventoryUI.isActiveAndEnabled)
            LoadData(playerInventoryScript.hotbarData, hotbarInventoryUI);

        if (playerInventoryUI.isActiveAndEnabled)
            LoadData(playerInventoryScript.inventoryData, playerInventoryUI);

        return remaining;
    }
    
    public bool TryRemoveItemsFromPlayerData(ItemData itemData, int amount)
    {
        if (itemData == null || amount <= 0) return false;

        int remaining = amount;
        remaining -= RemoveFromInventoryList(itemData, remaining,
            playerInventoryScript.hotbarData);

        if (remaining > 0)
            remaining -= RemoveFromInventoryList(itemData, remaining,
                playerInventoryScript.inventoryData);

        if (hotbarInventoryUI.isActiveAndEnabled)
            LoadData(playerInventoryScript.hotbarData, hotbarInventoryUI);

        if (playerInventoryUI.isActiveAndEnabled)
            LoadData(playerInventoryScript.inventoryData, playerInventoryUI);

        return remaining == 0;
    }

    int RemoveFromInventoryList(ItemData data, int request, InventoryData inv)
    {
        int before = request;
        TryRemoveItemsFromInventoryData(data, request, inv);
        return before - request;   // removed count
    }
    
    public bool TryRemoveItemsFromInventoryData(ItemData itemData, int amount, InventoryData targetInventoryData)
    {
        int remaining = amount;

        // First, collect all slots that contain the item
        List<int> matchingSlots = new List<int>();
        foreach (var kvp in targetInventoryData.inventoryData)
        {
            if (kvp.Value.item == itemData)
            {
                matchingSlots.Add(kvp.Key);
            }
        }

        // Sort by slot index for consistency
        matchingSlots.Sort();
        
        foreach (int index in matchingSlots)
        {
            if (remaining <= 0) break;

            ItemStack item = targetInventoryData.inventoryData[index];
            if (item.amount <= remaining)
            {
                remaining -= item.amount;
                targetInventoryData.inventoryData[index] = new ItemStack(); // Clear the slot
            }
            else
            {
                targetInventoryData.inventoryData[index] = new ItemStack { item = itemData, amount = item.amount - remaining };
                remaining = 0;
            }
        }

        // Refresh UI if it's open
        if (playerInventoryUI.isActiveAndEnabled)
        {
            LoadData(targetInventoryData, playerInventoryUI);
        }

        return remaining == 0; // true if we removed the full amount
    }


    
    public int IndexOfFirstOrEmptySlotData(InventoryData inventoryData, int startIndex = 0, ItemData itemToTransfer = null)
    {
        for (int i = startIndex; i < inventoryData.inventorySize; i++)
        {   
            if (inventoryData.inventoryData[i].item == itemToTransfer)
            {
                return i;
            }
            if (inventoryData.inventoryData[i].item == null)
            {
                return i;
            }
        }
        return -1;
    }

    public Dictionary<ItemData, int> GetCombinedInventory(InventoryData inventoryData)
    {
        Dictionary<ItemData, int> combinedItems = new Dictionary<ItemData, int>();

        foreach (var item in inventoryData.inventoryData)
        {
            ItemData itemData = item.Value.item;
            if(itemData == null)
                continue;
            if (itemData.name != "MissingItem")
            {
                if (!combinedItems.ContainsKey(itemData))
                {
                    combinedItems.Add(itemData, item.Value.amount);
                }
                else
                {
                    combinedItems[itemData] += item.Value.amount;
                }
            }
        }
        return combinedItems;
    }

    public bool DoesInventoryHaveItems(InventoryData inventoryData, List<ItemStack> itemStackss)
    {
        Dictionary<ItemData, int> combinedItems = GetCombinedInventory(inventoryData);

        foreach (var input in itemStackss)
        {
            ItemData item = input.item;
            if (!combinedItems.ContainsKey(item))
            {
                return false;
            }

            if (input.amount > combinedItems[item])
            {

                return false;
            }
        }
        return true;
    }
    
    public bool DoesPlayerHaveItems(List<ItemStack> required)
    {
        // Combine main inventory and hot-bar into one tally
        Dictionary<ItemData, int> combined = new Dictionary<ItemData, int>();

        void AddToCombined(Dictionary<ItemData, int> src)
        {
            foreach (var kv in src)
            {
                if (kv.Key == null) continue;

                if (combined.ContainsKey(kv.Key))
                    combined[kv.Key] += kv.Value;
                else
                    combined[kv.Key]  = kv.Value;
            }
        }

        AddToCombined(GetCombinedInventory(playerInventory.inventoryData));
        AddToCombined(GetCombinedInventory(playerInventory.hotbarData));

        // Check every required stack against the combined tally
        foreach (var need in required)
        {
            if (need.item == null || need.amount <= 0) continue;

            if (!combined.TryGetValue(need.item, out int have) || have < need.amount)
                return false;                       // missing or not enough
        }
        return true;
    }
    
    public bool DoesInventoryHaveItem(InventoryData inventoryData, ItemStack itemStack)
    {
        if (itemStack.amount <= 0) return false;

        Dictionary<ItemData, int> combined = GetCombinedInventory(inventoryData);

        ItemData item = itemStack.item;
        if (item == null)
            return false;
        combined.TryGetValue(item, out int amount);
        return amount >= itemStack.amount; 
    }

    
    public void DropItemPlayer(ItemStack item, Vector3 position)
    {
        if (item.item != null || item.amount != 0)
        {
            Vector3 positionOffset = Random.insideUnitCircle / 2;

            positionOffset.z = 0;
            positionOffset.y -= 0.15f;
            GameObject dropped = Instantiate(droppedItem, position + positionOffset,
                Quaternion.identity);

            DroppedItem droppedScript = dropped.GetComponent<DroppedItem>();
            if (droppedScript != null)
            {
                droppedScript.SetItem(item);
                droppedScript.canPickup = false;
            }
            
        }
    }
}
