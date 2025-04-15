using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Chest : MonoBehaviour
{
    public string chestID;
    public InventoryData inventoryData;
    public GameObject UI;
    public int chestSize;
    public bool inventoryOpen;
    public PlayerInventory playerInventory;
    
    private void Awake()
    {
        if (string.IsNullOrEmpty(chestID))
        {
            chestID = System.Guid.NewGuid().ToString(); // Unique chest ID
        }
        inventoryData = new InventoryData();
        PopulateChestWithRandomItems(chestSize / 2);

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
        if (InventoryManager.instance.currentlyInteractedObject != gameObject && InventoryManager.instance.currentlyInteractedObject != null)
        {
            InventoryManager.instance.currentlyInteractedObject.GetComponent<Chest>().CloseInventory();
        }
        if (!inventoryOpen)
        {
            UI.SetActive(true);
            playerInventory.OpenInventory();
            InventoryUI inventoryUI = UI.GetComponentInChildren<InventoryUI>();
            InventoryManager.instance.currentlyOpenedInventory = UI;
            InventoryManager.instance.currentlyInteractedObject = gameObject;

            if (inventoryUI != null)
            {
                inventoryUI.LoadData(inventoryData);
            }
            else
            {
                Debug.LogWarning("No Inventory UI found");
            }
            inventoryOpen = true;
        }
    }

    public void CloseInventory()
    {
        if (inventoryOpen)
        {
            UI.SetActive(false);
            InventoryUI inventoryUI = UI.GetComponentInChildren<InventoryUI>();
            InventoryManager.instance.currentlyOpenedInventory = null;
            InventoryManager.instance.currentlyInteractedObject = null;
            
            if (inventoryUI != null)
            {
                inventoryUI.SaveData(inventoryData);
            }
            inventoryOpen = false;

        }
    }
}
