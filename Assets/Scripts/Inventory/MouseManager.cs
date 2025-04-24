using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            int interactableLayer = LayerMask.GetMask("Interactable");
            
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, math.INFINITY, interactableLayer);
            if (hit.collider != null)
            {
                var chest = hit.collider.GetComponent<Chest>();
                if (chest != null)
                {
                    if (InventoryManager.instance.currentlyOpenedInventory == null)
                    {
                        chest.OpenInventory();
                    }
                }
                
                var furnace = hit.collider.GetComponent<Furnace>();
                if (furnace != null)
                {
                    if (InventoryManager.instance.currentlyOpenedInventory == null)
                    {
                        furnace.OpenFurnace();
                    }
                }
                
                var workbench = hit.collider.GetComponent<Workbench>();
                if (workbench != null)
                {
                    {
                        if (InventoryManager.instance.currentlyOpenedInventory == null)
                        {
                            workbench.OpenWorkbench();
                        }
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            int interactableLayer = LayerMask.GetMask("Interactable");

            
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, math.INFINITY, interactableLayer);
            if (hit.collider != null)
            {
                var building = hit.collider.GetComponent<Building>();
                if (building != null && !building.isGhost)
                {
                    building.Remove();
                }
                var staticBuilding = hit.collider.GetComponent<staticObject>();
                if (staticBuilding != null)
                {
                    staticBuilding.Remove();
                }
            }
        }
    }
}
