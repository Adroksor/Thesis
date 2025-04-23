using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workbench : MonoBehaviour
{
    public Building building;

    
    public void OpenWorkbench()
    {
        if (!building.isGhost)
        {
            InventoryManager.instance.playerInventory.OpenInventory();
            InventoryManager.instance.WorkbenchUI.SetActive(true);
            InventoryManager.instance.currentlyInteractedObject = gameObject;
            
            WorkbenchUI workbenchUI = InventoryManager.instance.WorkbenchUI.GetComponent<WorkbenchUI>();
            workbenchUI.InitializeRecipes();
        }
    }

    public void CloseWorkbench()
    {
        InventoryManager.instance.WorkbenchUI.SetActive(false);
        InventoryManager.instance.currentlyInteractedObject = null;
    }
}
