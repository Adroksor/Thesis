using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FurnaceInventory
{
    public List<ItemDataID> inputSlots;
    public List<ItemDataID> outputSlots;
    public int inputSlotCount;
    public int outputSlotCount;

    public FurnaceInventory(int inputCount, int outputCount)
    {
        inputSlotCount = inputCount;
        outputSlotCount = outputCount;

        inputSlots = new List<ItemDataID>(inputCount);
        outputSlots = new List<ItemDataID>(outputCount);

        for (int i = 0; i < inputCount; i++)
            inputSlots.Add(new ItemDataID { name = null, amount = 0 });

        for (int i = 0; i < outputCount; i++)
            outputSlots.Add(new ItemDataID { name = null, amount = 0 });
    }
    
    public void ClearInputs()
    {
        for (int i = 0; i < inputSlots.Count; i++)
            inputSlots[i] = new ItemDataID { name = null, amount = 0 };
    }

    public void ClearOutputs()
    {
        for (int i = 0; i < outputSlots.Count; i++)
            outputSlots[i] = new ItemDataID { name = null, amount = 0 };
    }


    public void SetInput(int slot, string name, int amount)
    {
        if (slot >= 0 && slot < inputSlotCount && !string.IsNullOrEmpty(name) && amount > 0)
            inputSlots[slot] = new ItemDataID { name = name, amount = amount };
    }

    public void SetOutput(int slot, string name, int amount)
    {
        if (slot >= 0 && slot < outputSlotCount && !string.IsNullOrEmpty(name) && amount > 0)
            outputSlots[slot] = new ItemDataID { name = name, amount = amount };
    }

    public void AddToOutput(string name, int amount)
    {
        for (int i = 0; i < outputSlots.Count; i++)
        {
            var slot = outputSlots[i]; // Copy the struct
            if (slot.name == name)
            {
                slot.amount += amount;
                outputSlots[i] = slot; // Reassign the modified copy back to the list
                return;
            }
        }

        for (int i = 0; i < outputSlots.Count; i++)
        {
            if (string.IsNullOrEmpty(outputSlots[i].name))
            {
                outputSlots[i] = new ItemDataID { name = name, amount = amount };
                return;
            }
        }

        Debug.LogWarning("No available output slot for item: " + name);
    }

    public void SubtractFromInput(string name, int amount)
    {
        for (int i = 0; i < outputSlots.Count; i++)
        {
            var slot = outputSlots[i];
            if (string.IsNullOrEmpty(slot.name))
            {
                slot.name = name;
                slot.amount = amount;
                outputSlots[i] = slot;
                return;
            }
        }
    }

    public void Clear()
    {
        for (int i = 0; i < inputSlots.Count; i++)
            inputSlots[i] = new ItemDataID { name = null, amount = 0 };

        for (int i = 0; i < outputSlots.Count; i++)
            outputSlots[i] = new ItemDataID { name = null, amount = 0 };
    }
}
