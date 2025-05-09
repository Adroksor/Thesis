using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingPlacer : MonoBehaviour
{
    public static BuildingPlacer instance;
    
    public GameObject buildingsList;

    private WorldGenerator worldGenerator;

    public GameObject selectedBuilding;

    public GameObject ghostBuilding;
    
    [SerializeField]
    public Chunk currentChunk;
    
    public Vector2Int mouseGridPosition;
    
    private Vector2Int lastMouseGridPosition;

    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    
    void Start()
    {
        if (BuildingGrid.instance == null)
        {
            Debug.LogError("BuildingGrid not found in the scene!");
        }
    }

    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseGridPosition = new Vector2Int(Mathf.FloorToInt(mousePosition.x + .5f), Mathf.FloorToInt(mousePosition.y + .5f));
        
        if (selectedBuilding != null)
        {
            UpdateGhostPosition();
        }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            selectedBuilding = null;
            Destroy(ghostBuilding);

        }

        if (Input.GetMouseButtonDown(0))
        {
            if (selectedBuilding != null)
            {
                PlayerInventory playerInventory = InventoryManager.instance.playerInventory;
                ItemStack item = new ItemStack
                {
                    item = ItemDatabaseInstance.instance.GetItemByname(selectedBuilding.name),
                    amount = 1
                };
                if (InventoryManager.instance.DoesInventoryHaveItem(playerInventory.inventoryData, item))
                {
                    bool placed = PlaceBuilding(selectedBuilding);
                    if (placed)
                    {
                        InventoryManager.instance.TryRemoveItemsFromInventoryData(ItemDatabaseInstance.instance.GetItemByname(item.item.name), 1, playerInventory.inventoryData);
                        if (!InventoryManager.instance.DoesInventoryHaveItem(playerInventory.inventoryData, item))
                        {
                            selectedBuilding = null;
                            Destroy(ghostBuilding);
                        }   
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector2Int currentChunkPosition = BuildingGrid.instance.WorldToChunkPosition(mouseGridPosition);
            currentChunk = WorldGenerator.instance.TryGetChunk(currentChunkPosition);
        }

        
        lastMouseGridPosition = mouseGridPosition;
    }

    public bool PlaceBuilding(GameObject buildingPrefab)
    {
        Building buildingB = buildingPrefab.GetComponent<Building>();
        
        if (BuildingGrid.instance.CanPlace(mouseGridPosition, buildingB))
        {
            GameObject buildingOBJ =  Instantiate(buildingPrefab, buildingsList.transform);
            buildingOBJ.name = buildingPrefab.name;
            Building building = buildingOBJ.GetComponent<Building>();
            building.isGhost = false;
            building.Place(mouseGridPosition);
            return true;
        }
        Debug.Log("Cannot place building here!");
        return false;
    }
    
    
    public GameObject PlaceBuildingFromSave(GameObject buildingPrefab, Vector2Int gridPosition)
    {
        GameObject buildingOBJ = Instantiate(buildingPrefab, buildingsList.transform);
        buildingOBJ.name = buildingPrefab.name;
        Building building = buildingOBJ.GetComponent<Building>();
        building.isGhost = false;
        building.Place(gridPosition);
        return buildingOBJ;
    }

    public void UpdateGhostPosition()
    {
        if(ghostBuilding == null) return;
        
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseGridPosition = new Vector2Int(Mathf.FloorToInt(mousePosition.x + .5f), Mathf.FloorToInt(mousePosition.y + .5f));
        
        ghostBuilding.transform.position = new Vector3(mouseGridPosition.x, mouseGridPosition.y, 0);
        
        SpriteRenderer sprite = ghostBuilding.GetComponent<SpriteRenderer>();
        
        if (BuildingGrid.instance.CanPlace(mouseGridPosition, ghostBuilding.GetComponent<Building>()))
        {
            sprite.color = new Color32(0, 255, 0, 128);
        }
        else
        {
            sprite.color = new Color32(255, 0, 0, 128);
        }
    }

    public void UpdateGhostObject(GameObject ghost = null)
    {
        if (ghostBuilding != null)
        {
            Destroy(ghostBuilding);
        }

        if (ghost != null)
        {
            selectedBuilding = ghost;
        }
        
        ghostBuilding = Instantiate(selectedBuilding);
        ghostBuilding.GetComponent<Building>().isGhost = true;
        ghostBuilding.GetComponent<Collider2D>().enabled = false;
        ghostBuilding.name = "GhostBuilding";
        SpriteRenderer sprite = ghostBuilding.GetComponent<SpriteRenderer>();
        sprite.color = new Color32(255, 255, 255, 128);
        sprite.sortingOrder = 10;
    }

    public void DestroyGhostBuilding()
    {
        Destroy(ghostBuilding);
    }

}