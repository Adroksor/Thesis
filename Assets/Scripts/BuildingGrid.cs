using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class BuildingGrid : MonoBehaviour
{
    public Tilemap groundTilemap; // Reference to the ground Tilemap
    public int chunkSize = 16; // Size of each chunk (e.g., 16x16 tiles)

    private Dictionary<Vector2Int, Chunk> chunks; // Dictionary to store all chunks
    

    void Start()
    {
        // Initialize the dictionary
        chunks = new Dictionary<Vector2Int, Chunk>();
    }

    // Get the chunk at a specific position
    public Chunk GetChunk(Vector2Int chunkPosition)
    {
        if (!chunks.ContainsKey(chunkPosition))
        {
            // Create a new chunk if it doesn't exist
            chunks[chunkPosition] = new Chunk(chunkPosition, chunkSize);
        }
        return chunks[chunkPosition];
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
        Chunk chunk = GetChunk(chunkPosition);


        return chunk.occupiedTiles.Contains(worldPosition);
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
    public void OccupyArea(Vector2Int worldPosition, Vector2Int size)
    {
        for (int x = worldPosition.x; x < worldPosition.x + size.x; x++)
        {
            for (int y = worldPosition.y; y < worldPosition.y + size.y; y++)
            {
                Vector2Int chunkPosition = WorldToChunkPosition(new Vector2Int(x, y));
                Chunk chunk = GetChunk(chunkPosition);


                chunk.occupiedTiles.Add(new Vector2Int(x, y)); // Mark tile as occupied
                foreach (Vector2 position in chunk.occupiedTiles)
                {
                    Debug.Log($"position: {position}");
                }
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
                Chunk chunk = GetChunk(chunkPosition);

                Vector2Int localPosition = new Vector2Int(
                    x - chunkPosition.x * chunkSize,
                    y - chunkPosition.y * chunkSize
                );

                chunk.occupiedTiles.Remove(localPosition); // Free the tile
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
}