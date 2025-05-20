using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class Chunk
{
    public Vector2Int position;
    public TileBase[,] tiles;
    public Dictionary<Vector2Int, GameObject> occupiedTiles;
    public readonly List<ResourceChange> changes;
    public bool isLoaded;
    public bool tilesSpawned;
    public bool resourcesSpawned = false;
    public GameObject chunkOBJ;

    public Chunk(Vector2Int position, int size, GameObject chunkList)
    {
        changes = new List<ResourceChange>();
        this.position = position;
        occupiedTiles = new Dictionary<Vector2Int, GameObject>();            
        tiles = new TileBase[size, size];
        isLoaded = true;
        tilesSpawned = false;
        chunkOBJ = new GameObject($"Chunk {position}");
        chunkOBJ.transform.position = new Vector3(position.x * size, position.y * size, 0);
        if (chunkList != null)
        {
            chunkOBJ.transform.parent = chunkList.transform;
        }
        else
        {
            Debug.LogWarning("ChunkList not found");
        }
    }

    public TileBase GetRandomTile(out int x, out int y)
    {
        int width  = tiles.GetLength(0);   // chunkSize in X
        int height = tiles.GetLength(1);   // chunkSize in Y

        x = UnityEngine.Random.Range(0, width);
        y = UnityEngine.Random.Range(0, height);

        return tiles[x, y];
    }
    
    public void RegisterRemoval(Vector2Int tile)
    {
        changes.Add(new ResourceChange { tile = tile, type = ChangeType.Removed });
    }
}