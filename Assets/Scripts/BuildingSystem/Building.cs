using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Building : MonoBehaviour
{
    public Vector2Int size = new Vector2Int(1, 1); // Size of the building (in tiles)
    public bool isBiomeBased = false;
    public bool placableOnWater = false;
    public TileBase[] allowedTiles; // Tiles the building can be placed on (e.g., water for pumps)

    public List<ItemDataID> drop;
    public GameObject itemPrefab;
    public bool isGhost = true;
    
    private Vector2Int gridPosition; // Position of the building on the grid

    public InventoryData internalInventory;

    public Action<Building> gettingRemoved;

    void Start()
    {
        itemPrefab = InventoryManager.instance.droppedItem;
    }


    // Place the building at a specific position
    public void Place(Vector2Int position)
    {
        gridPosition = position;
        BuildingGrid.instance.OccupyArea(position, this);
        transform.position = new Vector3(position.x, position.y, 0);
    }

    // Remove the building from the grid
    public void Remove()
    {
        gettingRemoved?.Invoke(this);
        BuildingGrid.instance.FreeArea(gridPosition, size);
        DropItems(drop);
        DropInternalItems();
        Destroy(gameObject);
    }

    public void DropItems(List<ItemDataID> itemsToDrop)
    {
        foreach (var item in itemsToDrop)
        {
            for(int i = 0; i < item.amount; i++)
            {
                if (item.name != null || item.amount != 0)
                {
                    Vector3 positionOffset = Random.insideUnitCircle / 2;

                    positionOffset.z = 0;
                    positionOffset.y -= 0.15f;
                    GameObject dropped = Instantiate(itemPrefab, transform.position + positionOffset,
                        Quaternion.identity);

                    DroppedItem droppedScript = dropped.GetComponent<DroppedItem>();
                    if (droppedScript != null)
                    {
                        ItemDataID toSet = new ItemDataID();
                        toSet.name = item.name;
                        toSet.amount = 1;
                        droppedScript.SetItem(toSet);
                    }
                }
            }
        }
    }

    public void DropInternalItems()
    {
        if (internalInventory == null) return;
        if (internalInventory.inventoryData != null)
        {
            foreach (var (key, value) in internalInventory.inventoryData)
            {
                if (value.name != null || value.amount != 0)
                {
                    Vector3 positionOffset = Random.insideUnitCircle / 2;

                    positionOffset.z = 0;
                    positionOffset.y -= 0.15f;
                    GameObject dropped = Instantiate(itemPrefab, transform.position + positionOffset,
                        Quaternion.identity);

                    DroppedItem droppedScript = dropped.GetComponent<DroppedItem>();
                    if (droppedScript != null)
                    {

                        droppedScript.SetItem(value);
                    }
                }
            }
        }
    }

    public void DropItem(ItemDataID item)
    {
        if (item.name != null || item.amount != 0)
        {
            Vector3 positionOffset = Random.insideUnitCircle / 2;

            positionOffset.z = 0;
            positionOffset.y -= 0.15f;
            GameObject dropped = Instantiate(itemPrefab, transform.position + positionOffset,
                Quaternion.identity);

            DroppedItem droppedScript = dropped.GetComponent<DroppedItem>();
            if (droppedScript != null)
            {

                droppedScript.SetItem(item);
            }
        }
    }
    
    public void Save(ref BuildingPositionData data)
    {
        data.position = transform.position;
        data.buildingName = transform.name;
    }

    public void Load(BuildingPositionData data)
    {
        transform.position = data.position;
    }
}