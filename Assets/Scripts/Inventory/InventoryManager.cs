using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventoryUI playerInventory;
    public InventoryUI hotbarInventory;
    public InventoryUI equipmentInventory;

    public InventorySlotUI initialSlot;
    public ItemData draggedItem;
    public int draggedItemCount;
    public static InventoryManager instance;
    
    [Header("OpenInventory")]
    public GameObject currentlyOpenedInventory;
    public GameObject currentlyInteractedObject;
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
        SubscribeSlotsToEvents(playerInventory.slots);
        SubscribeSlotsToEvents(hotbarInventory.slots);
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
                destinationInventory = hotbarInventory;
            else
                destinationInventory = currentlyOpenedInventory.GetComponentInChildren<InventoryUI>();

            // Avoid moving to the same inventory
            if (destinationInventory == inventory)
                destinationInventory = playerInventory;

            int index = IndexOfFirstEmptySlot(destinationInventory, 0, clickedItem.Name);

            while (clickedItemCount > 0 && index != -1)
            {
                InventorySlotUI targetSlot = destinationInventory.slots[index];

                int targetCount = targetSlot.itemCount;
                int spaceAvailable = clickedItem.MaxStackSize - targetCount;

                if (targetSlot.itemData == null || targetSlot.itemData == clickedItem)
                {
                    int moveAmount = Mathf.Min(spaceAvailable, clickedItemCount);

                    targetSlot.SetData(clickedItem, targetCount + moveAmount);
                    clickedItemCount -= moveAmount;

                    if (clickedItemCount > 0)
                    {
                        index = IndexOfFirstEmptySlot(destinationInventory, index + 1, clickedItem.Name);
                    }
                }
                else
                {
                    // Should never happen with a proper IndexOfFirstEmptySlot, but safe guard
                    index = IndexOfFirstEmptySlot(destinationInventory, index + 1, clickedItem.Name);
                }
            }

            // Update original slot
            if (clickedItemCount == 0)
                slot.SetData(null, 0);
            else
                slot.SetData(clickedItem, clickedItemCount);
        }
    }

    public int IndexOfFirstEmptySlot(InventoryUI inventoryUI, int startIndex = 0, string itemToTransfer = null)
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
}
