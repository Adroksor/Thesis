
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
        if (inventoryData.inventoryData == null)
        {
            inventoryData = new InventoryData(27);
        }

        if (hotbarData.inventoryData == null)
        {
            hotbarData = new InventoryData(9);
            hotbarData.SetData(1, new ItemStack{item = ItemDatabaseInstance.instance.GetItemByname("WoodPickaxe"), amount = 1});
            hotbarData.SetData(2, new ItemStack{item = ItemDatabaseInstance.instance.GetItemByname("Workbench"), amount = 1});
            hotbarData.SetData(3, new ItemStack{item = ItemDatabaseInstance.instance.GetItemByname("Furnace"), amount = 1});
            hotbarData.SetData(4, new ItemStack{item = ItemDatabaseInstance.instance.GetItemByname("Berries"), amount = 10});
            hotbarData.SetData(5, new ItemStack{item = ItemDatabaseInstance.instance.GetItemByname("IronChest"), amount = 5});
            hotbarData.SetData(7, new ItemStack{item = ItemDatabaseInstance.instance.GetItemByname("WoodWall"), amount = 10});
            hotbarData.SetData(8, new ItemStack{item = ItemDatabaseInstance.instance.GetItemByname("StoneWall"), amount = 10});
        }

        
        
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
