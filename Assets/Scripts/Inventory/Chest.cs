using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public string chestID;
    public InventoryData inventoryData;
    public GameObject UI;
    
    private void Awake()
    {
        if (string.IsNullOrEmpty(chestID))
        {
            chestID = System.Guid.NewGuid().ToString(); // Unique chest ID
        }
    }

    public void OpenInventory()
    {
        UI.SetActive(true);
        InventoryUI inventoryUI = UI.GetComponentInChildren<InventoryUI>();
        if (inventoryUI != null)
        {
            
            inventoryUI.LoadData(inventoryData);
        }
        else
        {
            Debug.LogWarning("No Inventory UI found");
        }
    }
}
