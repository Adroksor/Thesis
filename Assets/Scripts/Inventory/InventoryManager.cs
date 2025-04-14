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
}
