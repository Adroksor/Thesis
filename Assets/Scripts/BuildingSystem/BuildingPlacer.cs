using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingPlacer : MonoBehaviour
{
    public static BuildingPlacer instance;
    
    public GameObject buildingsList;

    public GameObject[] buildingPrefabs;
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

        if (mouseGridPosition != lastMouseGridPosition)
        {
            if (selectedBuilding != null)
            {
                UpdateGhostPosition();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedBuilding = buildingPrefabs[0];
            UpdateGhostObject();
            UpdateGhostPosition();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedBuilding = buildingPrefabs[1];
            UpdateGhostObject();
            UpdateGhostPosition();

        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedBuilding = buildingPrefabs[2];
            UpdateGhostObject();
            UpdateGhostPosition();

        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedBuilding = buildingPrefabs[3];
            UpdateGhostObject();
            UpdateGhostPosition();

        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            selectedBuilding = buildingPrefabs[4];
            UpdateGhostObject();
            UpdateGhostPosition();

        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            selectedBuilding = buildingPrefabs[5];
            UpdateGhostObject();
            UpdateGhostPosition();

        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            selectedBuilding = buildingPrefabs[6];
            UpdateGhostObject();
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
                ItemDataID item = new ItemDataID
                {
                    name = selectedBuilding.name,
                    amount = 1
                };
                if (InventoryManager.instance.DoesInventoryHaveItem(playerInventory.inventoryData, item))
                {
                    PlaceBuilding(selectedBuilding);
                    InventoryManager.instance.TryRemoveItemsFromInventoryData(ItemDatabaseInstance.instance.GetItemByname(item.name), 1, playerInventory.inventoryData);
                    if (!InventoryManager.instance.DoesInventoryHaveItem(playerInventory.inventoryData, item))
                    {
                        selectedBuilding = null;
                        Destroy(ghostBuilding);
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

    public void PlaceBuilding(GameObject buildingPrefab)
    {
        Vector2Int currentChunkPosition = BuildingGrid.instance.WorldToChunkPosition(mouseGridPosition);
        currentChunk = WorldGenerator.instance.TryGetChunk(currentChunkPosition);
        
        Building buildingP = buildingPrefab.GetComponent<Building>();
        
        if (BuildingGrid.instance.CanPlace(mouseGridPosition, buildingP))
        {
            GameObject buildingOBJ =  Instantiate(buildingPrefab, buildingsList.transform);
            Building building = buildingOBJ.GetComponent<Building>();
            building.isGhost = false;
            building.Place(mouseGridPosition);
            GameManager.instance.objects.Add(buildingOBJ);
        }
        else
        {
            Debug.Log("Cannot place building here!");
        }
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
        ghostBuilding.name = "GhostBuilding";
        SpriteRenderer sprite = ghostBuilding.GetComponent<SpriteRenderer>();
        sprite.color = new Color32(255, 255, 255, 128);
        sprite.sortingOrder = 10;
    }

    public GameObject GetObjectByName(string name)
    {
        return buildingPrefabs.FirstOrDefault(prefab => prefab.name == name);
    }
}