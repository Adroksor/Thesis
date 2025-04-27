using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FurnaceInventory
{
    public List<ItemStack> inputSlots;
    public List<ItemStack> outputSlots;
    public int inputSlotCount;
    public int outputSlotCount;

    public FurnaceInventory(int inputCount, int outputCount)
    {
        inputSlotCount = inputCount;
        outputSlotCount = outputCount;

        inputSlots = new List<ItemStack>(inputCount);
        outputSlots = new List<ItemStack>(outputCount);

        for (int i = 0; i < inputCount; i++)
            inputSlots.Add(new ItemStack { item = null, amount = 0 });

        for (int i = 0; i < outputCount; i++)
            outputSlots.Add(new ItemStack { item = null, amount = 0 });
    }
    
    public void ClearInputs()
    {
        for (int i = 0; i < inputSlots.Count; i++)
            inputSlots[i] = new ItemStack { item = null, amount = 0 };
    }

    public void ClearOutputs()
    {
        for (int i = 0; i < outputSlots.Count; i++)
            outputSlots[i] = new ItemStack { item = null, amount = 0 };
    }


    public void SetInput(int slot, ItemStack itemStack)
    {
        if (slot >= 0 && slot < inputSlotCount && !string.IsNullOrEmpty(itemStack.item.name) && itemStack.amount > 0)
            inputSlots[slot] = new ItemStack { item = itemStack.item, amount = itemStack.amount };
    }

    public void SetOutput(int slot, ItemStack itemStack)
    {
        if (slot >= 0 && slot < outputSlotCount && !string.IsNullOrEmpty(itemStack.item.name) && itemStack.amount > 0)
            outputSlots[slot] = new ItemStack { item = itemStack.item, amount = itemStack.amount };
    }

    public void AddToOutput(ItemStack itemStack)
    {
        for (int i = 0; i < outputSlots.Count; i++)
        {
            var slot = outputSlots[i];
            if (slot.item.name == itemStack.item.name)
            {
                slot.amount += itemStack.amount;
                outputSlots[i] = slot;
                return;
            }
        }

        for (int i = 0; i < outputSlots.Count; i++)
        {
            if (string.IsNullOrEmpty(itemStack.item.name))
            {
                outputSlots[i] = itemStack;
                return;
            }
        }

        Debug.LogWarning("No available output slot for item: " + itemStack.item.name);
    }

    public void SubtractFromInput(ItemStack itemStack)
    {
        for (int i = 0; i < inputSlots.Count; i++)
        {
            var slot = inputSlots[i];

            if (string.IsNullOrEmpty(itemStack.item.name))
            {
                if (slot.amount >= itemStack.amount)
                {
                    slot.amount -= itemStack.amount;
                    inputSlots[i] = slot;
                    return;
                }
                Debug.LogWarning($"Not enough {itemStack.item.name} in the input. Current amount: {slot.amount}, requested: {itemStack.amount}");
                return;
            }
        }
        Debug.LogWarning($"Item {itemStack.item.name} not found in input slots.");
    }

    public void Clear()
    {
        for (int i = 0; i < inputSlots.Count; i++)
            inputSlots[i] = new ItemStack { item = null, amount = 0 };

        for (int i = 0; i < outputSlots.Count; i++)
            outputSlots[i] = new ItemStack { item = null, amount = 0 };
    }
}
