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
    public bool chestOpen;
    
    private void Awake()
    {
        if (string.IsNullOrEmpty(chestID))
        {
            chestID = System.Guid.NewGuid().ToString(); // Unique chest ID
        }
        inventoryData = new InventoryData();
        Debug.Log(inventoryData.inventoryData.Count);
        inventoryData.SetData(Random.Range(0, 8), "Iron ingot", Random.Range(0, 8));
        inventoryData.SetData(Random.Range(0, 8), "Steel ingot", Random.Range(0, 8));
        inventoryData.SetData(Random.Range(0, 8), "Steel ingot", Random.Range(0, 8));

    }

    public void OpenInventory()
    {
        if (InventoryManager.instance.currentlyInteractedObject != gameObject && InventoryManager.instance.currentlyInteractedObject != null)
        {
            InventoryManager.instance.currentlyInteractedObject.GetComponent<Chest>().CloseInventory();
        }
        if (!chestOpen)
        {
            UI.SetActive(true);
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
            chestOpen = true;
        }
    }

    public void CloseInventory()
    {
        if (chestOpen)
        {
            UI.SetActive(false);
            InventoryUI inventoryUI = UI.GetComponentInChildren<InventoryUI>();
            InventoryManager.instance.currentlyOpenedInventory = null;
            if (inventoryUI != null)
            {
                inventoryUI.SaveData(inventoryData);
            }
            chestOpen = false;

        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseInventory();
        }
    }
}
