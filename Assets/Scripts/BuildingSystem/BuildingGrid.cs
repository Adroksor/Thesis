using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class BuildingGrid : MonoBehaviour
{
    public static BuildingGrid instance;
    public Tilemap groundTilemap; // Reference to the ground Tilemap
    public int chunkSize = 16; // Size of each chunk (e.g., 16x16 tiles)
    
    public static BuildingGrid Instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
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
    public bool CanPlace(Vector2Int position, Building building)
    {
        Vector2Int size = building.size;
        // Check if the building can be placed on the tiles
        for (int x = position.x; x < position.x + size.x; x++)
        {
            for (int y = position.y; y < position.y + size.y; y++)
            {
                Vector2Int tilePos = new Vector2Int(x, y);

                if (IsTileOccupied(tilePos)) return false;

                TileBase tile = GetTileAtPosition(tilePos);
                if (tile == null) return false;

                if (!building.placableOnWater && tile == WorldGenerator.instance.waterTile)
                    return false;

                if (building.isBiomeBased && !IsTileAllowed(tile, building))
                    return false;
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
        Chunk chunk = WorldGenerator.instance.TryGetChunk(chunkPosition);
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
    public void OccupyArea(Vector2Int worldPosition, Building building)
    {
        for (int x = worldPosition.x; x < worldPosition.x + building.size.x; x++)
        {
            for (int y = worldPosition.y; y < worldPosition.y + building.size.y; y++)
            {
                Vector2Int chunkPosition = WorldToChunkPosition(new Vector2Int(x, y));
                Chunk chunk = WorldGenerator.instance.TryGetChunk(chunkPosition);
                if (chunk != null)
                {
                    chunk.occupiedTiles.Add(new Vector2Int(x, y), building.gameObject); // Mark tile as occupied
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
                Chunk chunk = WorldGenerator.instance.TryGetChunk(chunkPosition);

                chunk.occupiedTiles.Remove(new Vector2Int(x, y)); // Free the tile
            }
        }
    }

    // Get the tile type at a position
    public TileBase GetTileAtPosition(Vector2Int worldPosition)
    {
        Vector2Int chunkPos = WorldToChunkPosition(worldPosition);
        Chunk chunk = WorldGenerator.instance.TryGetChunk(chunkPos);
        
        if (chunk != null)
        {
            Vector2Int localPos = new Vector2Int(
                worldPosition.x - chunkPos.x * chunkSize,
                worldPosition.y - chunkPos.y * chunkSize
            );

            if (localPos.x >= 0 && localPos.x < chunkSize && localPos.y >= 0 && localPos.y < chunkSize)
            {
                return chunk.tiles[localPos.x, localPos.y];
            }
        }

        return null;
    }

    public GameObject GetObjectAtPosition(Vector2Int worldPosition)
    {
        Vector2Int chunkPosition = WorldToChunkPosition(worldPosition);
        Chunk chunk = WorldGenerator.instance.TryGetChunk(chunkPosition);
        Vector2Int localPos = new Vector2Int(
            worldPosition.x - chunk.position.x * chunkSize,
            worldPosition.y - chunk.position.y * chunkSize);
        if (chunk.occupiedTiles.TryGetValue(worldPosition, out GameObject obj))
            return obj;
        return null;
    }
}