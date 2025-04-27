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
                var chest = hit.collider.GetComponentInParent<Chest>();
                if (chest != null)
                {
                    if (InventoryManager.instance.currentlyOpenedInventory == null)
                    {
                        chest.OpenInventory();
                    }
                }
                
                var furnace = hit.collider.GetComponentInParent<Furnace>();
                if (furnace != null)
                {
                    if (InventoryManager.instance.currentlyOpenedInventory == null)
                    {
                        furnace.OpenFurnace();
                    }
                }
                
                var workbench = hit.collider.GetComponentInParent<Workbench>();
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
                var building = hit.collider.GetComponentInParent<Building>();
                if (building != null && !building.isGhost)
                {
                    building.Remove();
                }
            }
        }
    }
}
