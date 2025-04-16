
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerInventory : MonoBehaviour
{
    public InventoryData inventoryData;
    public int inventorySize = 27;
    public GameObject UI;
    public bool inventoryOpen = false;

    private void Start()
    {
        inventoryData = new InventoryData(27);
        PopulateInventoryWithRandomItems(inventorySize / 2);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (inventoryOpen)
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inventoryOpen)
            {
                CloseInventory();
            }
        }
    }

    public void PopulateInventoryWithRandomItems(int amount)
    {
        string[] itemNames = {"Coal", "Iron ore", "Iron ingot", "Steel ingot" };
        int randomIndex;
        for (int i = 0; i < amount; i++)
        {
            randomIndex = Random.Range(0, itemNames.Length);
            inventoryData.SetData(Random.Range(0, inventorySize), itemNames[randomIndex], Random.Range(0, 8));
        }
    }

    public void OpenInventory()
    {
        if (!inventoryOpen)
        {
            UI.SetActive(true);
            InventoryUI inventoryUI = UI.GetComponentInChildren<InventoryUI>();
            
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

    public void CloseInventory()
    {
        UI.SetActive(false);
        InventoryUI inventoryUI = UI.GetComponentInChildren<InventoryUI>();
        if (inventoryUI != null)
        {
            InventoryManager.instance.SaveData(inventoryData, inventoryUI);
        }
        inventoryOpen = false;
        
        GameObject inventory2Object = InventoryManager.instance.currentlyInteractedObject;
        if (inventory2Object != null)
        {
            inventory2Object.TryGetComponent<Chest>(out Chest chest);
            if (chest != null)
            {
                chest.CloseInventory();
            }
        }
    }
}
