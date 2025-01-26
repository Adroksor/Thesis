using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingPlacer : MonoBehaviour
{
    public GameObject[] buildingPrefabs;
    private BuildingGrid buildingGrid;

    public GameObject selectedBuilding;

    public GameObject ghostBuilding;
    
    
    [SerializeField]
    public Chunk currentChunk;
    
    public Vector2Int mouseGridPosition;

    
    private Vector2Int lastMouseGridPosition;

    void Start()
    {
        buildingGrid = FindFirstObjectByType<BuildingGrid>();
        if (buildingGrid == null)
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
        if (Input.GetKeyDown(KeyCode.X))
        {
            selectedBuilding = null;
            UpdateGhostObject();

        }

        if (Input.GetMouseButtonDown(0))
        {
            if (selectedBuilding != null)
            {
                PlaceBuilding(selectedBuilding);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector2Int currentChunkPosition = buildingGrid.WorldToChunkPosition(mouseGridPosition);
            currentChunk = buildingGrid.GetChunk(currentChunkPosition);
            Debug.Log(currentChunk);
        }

        
        lastMouseGridPosition = mouseGridPosition;
    }

    public void PlaceBuilding(GameObject buildingPrefab)
    {
        Vector2Int currentChunkPosition = buildingGrid.WorldToChunkPosition(mouseGridPosition);
        currentChunk = buildingGrid.GetChunk(currentChunkPosition);
        
        if (buildingGrid.CanPlace(mouseGridPosition, buildingPrefab))
        {
            
            GameObject buildingObject = Instantiate(buildingPrefab);

            
            Building building = buildingObject.GetComponent<Building>();

            
            building.Place(mouseGridPosition);
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
        if (buildingGrid.CanPlace(mouseGridPosition, ghostBuilding))
        {
            sprite.color = new Color32(0, 255, 0, 128);
        }
        else
        {
            sprite.color = new Color32(255, 0, 0, 128);
        }
    }

    private void UpdateGhostObject()
    {
        
        if (ghostBuilding != null)
        {
            Destroy(ghostBuilding);
        }
        
        ghostBuilding = Instantiate(selectedBuilding);
        ghostBuilding.name = "GhostBuilding";
        SpriteRenderer sprite = ghostBuilding.GetComponent<SpriteRenderer>();
        sprite.color = new Color32(255, 255, 255, 128);
        sprite.sortingOrder = 10;
    }
}