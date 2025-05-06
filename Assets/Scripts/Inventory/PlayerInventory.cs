
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerInventory : MonoBehaviour
{
    public InventoryData inventoryData;
    public ItemUser itemUser;
    public GameObject UI;
    public bool inventoryOpen = false;

    public InventoryData hotbarData;

    private void Start()
    {
        inventoryData = new InventoryData(27);
        
        
        hotbarData = new InventoryData(9);

        hotbarData.SetData(0, new ItemStack{item = ItemDatabaseInstance.instance.GetItemByname("WoodPickaxe"), amount = 1});
        hotbarData.SetData(1, new ItemStack{item = ItemDatabaseInstance.instance.GetItemByname("StonePickaxe"), amount = 1});
        hotbarData.SetData(2, new ItemStack{item = ItemDatabaseInstance.instance.GetItemByname("IronPickaxe"), amount = 1});
        hotbarData.SetData(3, new ItemStack{item = ItemDatabaseInstance.instance.GetItemByname("Workbench"), amount = 1});
        hotbarData.SetData(4, new ItemStack{item = ItemDatabaseInstance.instance.GetItemByname("WoodChest"), amount = 1});
        hotbarData.SetData(5, new ItemStack{item = ItemDatabaseInstance.instance.GetItemByname("IronChest"), amount = 1});
        hotbarData.SetData(6, new ItemStack{item = ItemDatabaseInstance.instance.GetItemByname("GoldChest"), amount = 1});
        hotbarData.SetData(7, new ItemStack{item = ItemDatabaseInstance.instance.GetItemByname("Berries"), amount = 20});
        
        
        InventoryManager.instance.LoadData(hotbarData, InventoryManager.instance.hotbarInventoryUI);
        //PopulateInventoryWithRandomItems(20);
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
        string[] itemNames = {"OakLog", "IronOre", "Coal", "IronIngot", "Berries"};
        int randomIndex;
        for (int i = 0; i < amount; i++)
        {
            randomIndex = Random.Range(0, itemNames.Length);
            ItemStack stack = new ItemStack
            {
                item = ItemDatabaseInstance.instance.GetItemByname(itemNames[randomIndex]),
                amount = Random.Range(1, 8)
            };
            int random = Random.Range(0, inventoryData.inventorySize);
            inventoryData.SetData(random, stack);
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
            InventoryManager.instance.SaveEQandHotbar();
        }
        inventoryOpen = false;
        
        GameObject inventory2Object = InventoryManager.instance.currentlyInteractedObject;
        if (inventory2Object != null)
        {
            inventory2Object.TryGetComponent(out Chest chest);
            if (chest != null)
            {
                chest.CloseInventory();
            }
            inventory2Object.TryGetComponent(out Furnace furnace);
            if (furnace != null)
            {
                furnace.CloseFurnace();
            }
            inventory2Object.TryGetComponent(out Workbench workbench);
            if (workbench != null)
            {
                workbench.CloseWorkbench();
            }
        }
    }
}
