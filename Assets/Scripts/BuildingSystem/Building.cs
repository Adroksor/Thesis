using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Building : MonoBehaviour
{
    public Vector2Int size = new Vector2Int(1, 1);
    public bool isBiomeBased = false;
    public bool placableOnWater = false;
    public TileBase[] allowedTiles;

    public List<ItemStack> drop;
    public GameObject itemPrefab;
    public bool isGhost = true;
    
    public Vector2Int gridPosition;

    public InventoryData internalInventory;

    public  BuildingStats stats;
    public Action<Building> gettingRemoved;


    private void OnEnable()
    {
        stats = GetComponent<BuildingStats>();

        stats.destroyMe += Remove;
    }

    private void OnDisable()
    {
        stats.destroyMe -= Remove;
    }

    void Start()
    {
        itemPrefab = InventoryManager.instance.droppedItem;
        
    }


    public void PlaceAndOccupy(Vector2Int position)
    {
        gridPosition = position;
        BuildingGrid.instance.OccupyArea(position, this);
        transform.position = new Vector3(position.x, position.y, 0);
    }
    
    public void PlaceAndDontOccupy(Vector2Int position)
    {
        gridPosition = position;
        transform.position = new Vector3(position.x, position.y, 0);
    }

    public void Remove()
    {
        gettingRemoved?.Invoke(this);
        BuildingGrid.instance.FreeArea(gridPosition, size);
        DropItems(drop);
        DropInternalItems();

        Resource resource = GetComponent<Resource>();
        if (resource != null)
            resource.parentChunk.RegisterRemoval(gridPosition);
        
        Destroy(gameObject);
    }

    public void DropItems(List<ItemStack> itemsToDrop)
    {
        foreach (var item in itemsToDrop)
        {
            for(int i = 0; i < item.amount; i++)
            {
                if (!string.IsNullOrEmpty(item.item.name) || item.amount != 0)
                {
                    Vector3 positionOffset = Random.insideUnitCircle / 2;

                    positionOffset.z = 0;
                    positionOffset.y -= 0.15f;
                    GameObject dropped = Instantiate(itemPrefab, transform.position + positionOffset,
                        Quaternion.identity);

                    DroppedItem droppedScript = dropped.GetComponent<DroppedItem>();
                    if (droppedScript != null)
                    {
                        ItemStack toSet = new ItemStack
                        {
                            item = item.item,
                            amount = 1
                        };
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
                if (value.item == null)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(value.item.name))
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

    public void DropItem(ItemStack item)
    {
        if (!string.IsNullOrEmpty(item.item.name) || item.amount != 0)
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
}