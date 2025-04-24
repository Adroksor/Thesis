using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerInventory inventoryData;
    
    
    public void Save(ref PlayerData data)
    {
        data.position = transform.position;
        Debug.Log(data.position);
        
        List<SlotSaveData> inventory = inventoryData.inventoryData.ToSaveList();
        data.inventory = inventory;
        List<SlotSaveData> hotbar = inventoryData.hotbarData.ToSaveList();
        data.hotbar = hotbar;
        
    }

    public void Load(PlayerData data)
    {
        Debug.Log(data.position);

        gameObject.transform.position = data.position;
        
        inventoryData.inventoryData.FromSaveList(data.inventory);
        inventoryData.hotbarData.FromSaveList(data.hotbar);
        InventoryManager.instance.LoadData(inventoryData.hotbarData, InventoryManager.instance.hotbarInventoryUI);
        
    }
}
