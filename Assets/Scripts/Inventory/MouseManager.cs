using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
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
                        furnace.GivePlayerSmeltedItems();
                    }
                }
            }
        }
    }
}
