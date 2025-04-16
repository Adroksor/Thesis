using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Building : MonoBehaviour
{
    public Vector2Int size = new Vector2Int(1, 1); // Size of the building (in tiles)
    public bool isBiomeBased = false;
    public TileBase[] allowedTiles; // Tiles the building can be placed on (e.g., water for pumps)

    public List<ItemDataID> drop;
    public GameObject itemPrefab;
    
    
    private BuildingGrid buildingGrid; // Reference to the BuildingGrid
    private Vector2Int gridPosition; // Position of the building on the grid


    void Start()
    {
        // Find the BuildingGrid in the scene
        buildingGrid = BuildingGrid.instance;
        
        if (buildingGrid == null)
        {
            Debug.LogError("BuildingGrid not found in the scene!");
        }

        itemPrefab = InventoryManager.instance.droppedItem;
    }


    // Place the building at a specific position
    public void Place(Vector2Int position)
    {
        gridPosition = position;
        buildingGrid.OccupyArea(position, this);
        transform.position = new Vector3(position.x, position.y, 0);
        Debug.Log("Building placed successfully!");
    }

    // Remove the building from the grid
    public void Remove()
    {
        buildingGrid.FreeArea(gridPosition, size);
        DropItems(drop);
        Destroy(gameObject);
        Debug.Log("Building removed!");
    }

    public void DropItems(List<ItemDataID> itemsToDrop)
    {
        foreach (var item in itemsToDrop)
        {
            for(int i = 0; i < item.amount; i++)
            {
                Vector3 positionOffset = Random.insideUnitCircle / 2;

                positionOffset.z = 0;
                positionOffset.y -= 0.15f;
                GameObject dropped = Instantiate(itemPrefab, transform.position + positionOffset, Quaternion.identity);
            

                // Optional: pass item data to the dropped object
                DroppedItem droppedScript = dropped.GetComponent<DroppedItem>();
                if (droppedScript != null)
                {
                    Debug.Log("Dropped item: " + item.name);
                    droppedScript.SetItem(item); // Your method to assign item data
                }
            }
        }
    }
    
    
}