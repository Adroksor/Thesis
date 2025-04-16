using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public InventoryUI playerInventoryUI;
    public InventoryUI hotbarInventoryUI;
    public InventoryUI equipmentInventory;

    public List<InventoryUI> inventoriyUIs;

    public GameObject FurnaceUI;

    public PlayerInventory playerInventoryScript;
    
    public InventorySlotUI initialSlot;
    public ItemData draggedItem;
    public int draggedItemCount;
    public static InventoryManager instance;
    
    [Header("OpenInventory")]
    public InventoryUI currentlyOpenedInventory;
    public GameObject currentlyInteractedObject;

    public GameObject droppedItem;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    private void Start()
    {
        //InitializePlayerInventory();
    }
    
    public void InitializePlayerInventory()
    {
        SubscribeSlotsToEvents(playerInventoryUI.slots);
        SubscribeSlotsToEvents(hotbarInventoryUI.slots);
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
        SaveData(playerInventoryScript.inventoryData, playerInventoryUI);
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
        SaveData(playerInventoryScript.inventoryData, playerInventoryUI);
    }
    private void OnClick(InventorySlotUI slot, InventoryUI inventory)
    {
        ItemData clickedItem = slot.itemData;
        int clickedItemCount = slot.itemCount;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (clickedItem == null || clickedItemCount <= 0)
                return;

            InventoryUI destinationInventory;

            if (currentlyOpenedInventory == null)
                destinationInventory = hotbarInventoryUI;
            else
                destinationInventory = currentlyOpenedInventory.GetComponentInChildren<InventoryUI>();

            if (destinationInventory == inventory)
                destinationInventory = playerInventoryUI;

            int leftover = TryAddItemToInventory(clickedItem, clickedItemCount, destinationInventory);

            // Update original slot
            if (leftover == 0)
                slot.SetData(null, 0);
            else
                slot.SetData(clickedItem, leftover);
        }
    }
    
    public void SaveData(InventoryData inventoryData, InventoryUI inventoryUI)
    {
        inventoryData.inventoryData.Clear();
        
        foreach (InventorySlotUI slot in inventoryUI.slots)
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
            inventoryData.inventoryData.Add(inventoryUI.slots.IndexOf(slot), item);
        }
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
            inventoryUI.slots[key].SetData(ItemDatabaseInstance.Instance.GetItemByName(value.name), value.amount);
        }
    }

    
    public int TryAddItemToInventory(ItemData itemData, int amount, InventoryUI targetInventory)
    {
        if (itemData == null || amount <= 0 || targetInventory == null)
            return amount;

        int index = IndexOfFirstOrEmptySlot(targetInventory, 0, itemData.Name);

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
                    index = IndexOfFirstOrEmptySlot(targetInventory, index + 1, itemData.Name);
            }
            else
            {
                index = IndexOfFirstOrEmptySlot(targetInventory, index + 1, itemData.Name);
            }
        }
        return amount; // Return leftover amount
    }


    public int IndexOfFirstOrEmptySlot(InventoryUI inventoryUI, int startIndex = 0, string itemToTransfer = null)
    {
        for (int i = startIndex; i < inventoryUI.slots.Count; i++)
        {
            if (inventoryUI.slots[i].itemData ==null || inventoryUI.slots[i].itemData.Name == itemToTransfer)
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
            index = IndexOfFirstOrEmptySlotData(targetInventoryData, index + 1, itemData.Name);
            if (index == -1) return amount;
            int maxStackSize = itemData.MaxStackSize; 
            targetInventoryData.inventoryData.TryGetValue(index, out ItemDataID itemAtIndex);

            if (itemAtIndex.name == null)
            {
                // New stack
                int added = Mathf.Min(amount, maxStackSize);
                targetInventoryData.inventoryData[index] = new ItemDataID { name = itemData.Name, amount = added };
                amount -= added;
            }
            else if (itemAtIndex.name == itemData.Name && itemAtIndex.amount < maxStackSize)
            {
                // Add to existing stack
                int spaceLeft = maxStackSize - itemAtIndex.amount;
                int added = Mathf.Min(amount, spaceLeft);
                targetInventoryData.inventoryData[index] = new ItemDataID { name = itemData.Name, amount = itemAtIndex.amount + added };
                amount -= added;
            }
        }

        if (playerInventoryUI.isActiveAndEnabled)
        {
            LoadData(targetInventoryData, playerInventoryUI);
        }
        return amount;
    }

    public int IndexOfFirstOrEmptySlotData(InventoryData inventoryData, int startIndex = 0, string itemToTransfer = null)
    {
        for (int i = startIndex; i < inventoryData.inventorySize; i++)
        {
            if (inventoryData.inventoryData[i].name == itemToTransfer)
            {
                return i;
            }
            if (inventoryData.inventoryData[i].name == null)
            {
                return i;
            }
        }
        return -1;
    }
}
