using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class BuildingGrid : MonoBehaviour
{
    public Tilemap groundTilemap; // Reference to the ground Tilemap
    public int chunkSize = 16; // Size of each chunk (e.g., 16x16 tiles)
    
    public static BuildingGrid Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple BuildingGrid instances found!");
            Destroy(gameObject);
        }
    }
    void Start()
    {
        
    }

    // Convert world position to chunk position
    public Vector2Int WorldToChunkPosition(Vector2Int worldPosition)
    {
        return new Vector2Int(
            Mathf.FloorToInt((float)worldPosition.x / chunkSize),
            Mathf.FloorToInt((float)worldPosition.y / chunkSize)
        );
    }

    // Check if the building can be placed at a specific position
    public bool CanPlace(Vector2Int position, GameObject buildingOBJ)
    {
        Building building = buildingOBJ.GetComponent<Building>();
        Vector2Int size = building.size;
        
        // Check if the area is available
        if (!IsAreaAvailable(position, size))
        {
            return false;
        }

        if (!building.isBiomeBased)
        {
            return true;
        }
            
        // Check if the building can be placed on the tiles
        for (int x = position.x; x < position.x + size.x; x++)
        {
            for (int y = position.y; y < position.y + size.y; y++)
            {
                TileBase tile = GetTileAtPosition(new Vector2Int(x, y));
                if (tile == null || !IsTileAllowed(tile, building))
                {
                    Debug.Log("Building cannot be placed on this tile!");
                    return false;
                }
            }
        }

        return true; // Building can be placed
    }
    
    // Check if a tile is allowed for this building
    public bool IsTileAllowed(TileBase tile, Building building)
    {
        foreach (TileBase allowedTile in building.allowedTiles)
        {
            if (tile == allowedTile)
            {
                return true;
            }
        }
        return false;
    }
    
    // Check if a tile is occupied
    public bool IsTileOccupied(Vector2Int worldPosition)
    {
        Vector2Int chunkPosition = WorldToChunkPosition(worldPosition);
        Chunk chunk = WorldGenerator.GetChunkAt(chunkPosition);


        return chunk.occupiedTiles.ContainsKey(worldPosition);
    }

    // Check if an area is available for building
    public bool IsAreaAvailable(Vector2Int worldPosition, Vector2Int size)
    {
        for (int x = worldPosition.x; x < worldPosition.x + size.x; x++)
        {
            for (int y = worldPosition.y; y < worldPosition.y + size.y; y++)
            {
                if (IsTileOccupied(new Vector2Int(x, y)))
                {
                    return false; // Area is occupied
                }
            }
        }
        return true; // Area is available
    }

    // Occupy an area in the grid
    public void OccupyArea(Vector2Int worldPosition, Building buildingOBJ)
    {
        for (int x = worldPosition.x; x < worldPosition.x + buildingOBJ.size.x; x++)
        {
            for (int y = worldPosition.y; y < worldPosition.y + buildingOBJ.size.y; y++)
            {
                Vector2Int chunkPosition = WorldToChunkPosition(new Vector2Int(x, y));
                Chunk chunk = WorldGenerator.GetChunkAt(chunkPosition);

                chunk.occupiedTiles.Add(new Vector2Int(x, y), buildingOBJ.gameObject); // Mark tile as occupied
            }
        }
    }

    // Free an area in the grid
    public void FreeArea(Vector2Int worldPosition, Vector2Int size)
    {
        for (int x = worldPosition.x; x < worldPosition.x + size.x; x++)
        {
            for (int y = worldPosition.y; y < worldPosition.y + size.y; y++)
            {
                Vector2Int chunkPosition = WorldToChunkPosition(new Vector2Int(x, y));
                Chunk chunk = WorldGenerator.GetChunkAt(chunkPosition);
                
                chunk.occupiedTiles.Remove(new Vector2Int(x, y)); // Free the tile
            }
        }
    }

    // Get the tile type at a position
    public TileBase GetTileAtPosition(Vector2Int worldPosition)
    {
        if (groundTilemap != null)
        {
            return groundTilemap.GetTile(new Vector3Int(worldPosition.x, worldPosition.y, 0));
        }
        return null;
    }

    public GameObject GetObjectAtPosition(Vector2Int worldPosition)
    {
        Vector2Int chunkPosition = WorldToChunkPosition(worldPosition);
        Chunk chunk = WorldGenerator.GetChunkAt(chunkPosition);
        if (chunk.occupiedTiles.TryGetValue(worldPosition, out GameObject obj))
            return obj;
        return null;
    }
}