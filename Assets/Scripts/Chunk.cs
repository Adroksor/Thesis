using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class Chunk
{
    public Vector2Int position; // Chunk position in chunk coordinates
    public TileBase[,] tiles; // 2D array of tiles in the chunk
    public List<Vector2Int> occupiedTiles; // Dictionary to track occupied tiles in this chunk
    public bool isLoaded; // Whether the chunk is currently loaded
    public GameObject chunkOBJ;

    public Chunk(Vector2Int position, int size)
    {
        this.position = position;
        occupiedTiles = new List<Vector2Int>();            
        tiles = new TileBase[size, size];
        isLoaded = true;
        chunkOBJ = new GameObject($"Chunk {position}");
        chunkOBJ.transform.parent = GameObject.FindGameObjectWithTag("ChunkList").transform;
    }
}