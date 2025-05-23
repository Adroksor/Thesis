using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class BuildingGrid : MonoBehaviour
{
    public static BuildingGrid instance;
    public int chunkSize = 16;
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public Vector2Int WorldToChunkPosition(Vector2Int worldPosition)
    {
        return new Vector2Int(
            Mathf.FloorToInt((float)worldPosition.x / chunkSize),
            Mathf.FloorToInt((float)worldPosition.y / chunkSize)
        );
    }

    public bool CanPlace(Vector2Int position, Building building)
    {
        Vector2Int size = building.size;
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
        return true;
    }
    
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
    
    public bool IsTileOccupied(Vector2Int worldPosition)
    {
        Vector2Int chunkPosition = WorldToChunkPosition(worldPosition);
        Chunk chunk = WorldGenerator.instance.TryGetChunk(chunkPosition);
        return chunk.occupiedTiles.ContainsKey(worldPosition);
    }

    public bool IsAreaAvailable(Vector2Int worldPosition, Vector2Int size)
    {
        for (int x = worldPosition.x; x < worldPosition.x + size.x; x++)
        {
            for (int y = worldPosition.y; y < worldPosition.y + size.y; y++)
            {
                if (IsTileOccupied(new Vector2Int(x, y)))
                {
                    return false;
                }
            }
        }
        return true;
    }

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
                    chunk.occupiedTiles.Add(new Vector2Int(x, y), building.gameObject);
                }
            }
        }
    }

    public void FreeArea(Vector2Int worldPosition, Vector2Int size)
    {
        for (int x = worldPosition.x; x < worldPosition.x + size.x; x++)
        {
            for (int y = worldPosition.y; y < worldPosition.y + size.y; y++)
            {
                Vector2Int chunkPosition = WorldToChunkPosition(new Vector2Int(x, y));
                Chunk chunk = WorldGenerator.instance.TryGetChunk(chunkPosition);

                chunk.occupiedTiles.Remove(new Vector2Int(x, y));
            }
        }
    }
    
    public TileBase GetTileAtPosition(Vector2Int worldPosition)
    {
        var pos = new Vector3Int(worldPosition.x, worldPosition.y, 0);
        var tile = WorldGenerator.instance.groundLevel.GetTile(pos);
        if (tile != null)
            return tile;

        tile = WorldGenerator.instance.waterLevel.GetTile(pos);
        return tile;
    }

    public GameObject GetObjectAtPosition(Vector2Int worldPosition)
    {
        Vector2Int chunkPosition = WorldToChunkPosition(worldPosition);
        Chunk chunk = WorldGenerator.instance.TryGetChunk(chunkPosition);

        if (chunk.occupiedTiles.TryGetValue(worldPosition, out GameObject obj))
            return obj;
        return null;
    }
}