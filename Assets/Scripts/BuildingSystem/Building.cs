using UnityEngine;
using UnityEngine.Tilemaps;

public class Building : MonoBehaviour
{
    public Vector2Int size = new Vector2Int(1, 1); // Size of the building (in tiles)
    public bool isBiomeBased = false;
    public TileBase[] allowedTiles; // Tiles the building can be placed on (e.g., water for pumps)
    
    
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
        Destroy(gameObject);
        Debug.Log("Building removed!");
    }

}