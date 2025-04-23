using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Chest : MonoBehaviour
{
    public string Guid => guid;
    private string guid;
    public InventoryData inventoryData;
    public InventoryUI UI;
    public int chestSize;
    public bool inventoryOpen;
    public PlayerInventory playerInventory;

    public Building building;
    
    private void Awake()
    {
        inventoryData = new InventoryData(chestSize);
        PopulateChestWithRandomItems(chestSize / 2);

        SetChestUI();
        
        playerInventory = InventoryManager.instance.playerInventory;

        building.gettingRemoved += RemovingChest;
    }

    private void SetChestUI()
    {
        if (chestSize == 27)
        {
            UI = InventoryManager.instance.inventoriyUIs[0];
        }
        else if (chestSize == 18)
        {
            UI = InventoryManager.instance.inventoriyUIs[1];
        }
        else if (chestSize == 9)
        {
            UI = InventoryManager.instance.inventoriyUIs[2];
        }
        else
        {
            Debug.Log("Invalid chest size");
        }
    }

    public void PopulateChestWithRandomItems(int amount)
    {
        string[] itemNames = {"Coal", "Iron ore", "Iron ingot", "Steel ingot" };
        int randomIndex;
        for (int i = 0; i < amount; i++)
        {
            randomIndex = Random.Range(0, itemNames.Length);
            inventoryData.SetData(Random.Range(0, chestSize), itemNames[randomIndex], Random.Range(0, 8));
        }
    }

    public void OpenInventory()
    {
        if (!building.isGhost)
        {
            if (InventoryManager.instance.currentlyInteractedObject != gameObject && InventoryManager.instance.currentlyInteractedObject != null)
            {
                InventoryManager.instance.currentlyInteractedObject.TryGetComponent(out Chest chest);
                chest.CloseInventory();
            }
            if (!inventoryOpen)
            {
                UI.transform.parent.gameObject.SetActive(true);
                playerInventory.OpenInventory();
                InventoryUI inventoryUI = UI.GetComponentInChildren<InventoryUI>();
                InventoryManager.instance.currentlyOpenedInventory = UI;
                InventoryManager.instance.currentlyInteractedObject = gameObject;

                if (inventoryUI != null)
                {
                    InventoryManager.instance.LoadData(inventoryData, inventoryUI);
                }
                else
                {
                    Debug.LogWarning("No Inventory UI found");
                }
                inventoryOpen = true;
            }
        }
    }

    public void CloseInventory()
    {
        if (inventoryOpen)
        {            
            UI.transform.parent.gameObject.SetActive(false);
            InventoryUI inventoryUI = UI.GetComponentInChildren<InventoryUI>();
            InventoryManager.instance.currentlyOpenedInventory = null;
            InventoryManager.instance.currentlyInteractedObject = null;
            
            if (inventoryUI != null)
            {
                InventoryManager.instance.SaveData(inventoryData, inventoryUI);
            }
            inventoryOpen = false;

        }
    }

    public void RemovingChest(Building building)
    {
        CloseInventory();
        building.internalInventory = inventoryData;
    }
}


[Serializable] 
public class ChestSaveData {
    public string guid;
    public Vector2 position;
    public List<ItemDataID> inventory;   // your serialisable inventory list
}

public interface ISaveableObject {
    string Guid { get; }                     // stable ID you already generate
    object CaptureState();                   // returns a SaveData object
    void RestoreState(object state);         // injects SaveData back in
}