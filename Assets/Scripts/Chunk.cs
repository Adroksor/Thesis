using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class Chunk
{
    public Vector2Int position; // Chunk position in chunk coordinates
    public TileBase[,] tiles; // 2D array of tiles in the chunk
    public Dictionary<Vector2Int, GameObject> occupiedTiles; // Dictionary to track occupied tiles in this chunk
    public bool isLoaded; // Whether the chunk is currently loaded
    public GameObject chunkOBJ;

    public Chunk(Vector2Int position, int size, GameObject chunkList)
    {
        this.position = position;
        occupiedTiles = new Dictionary<Vector2Int, GameObject>();            
        tiles = new TileBase[size, size];
        isLoaded = true;
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
}