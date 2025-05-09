using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{

    public Chunk parentChunk;
    
    private void Start()
    {
        Vector2Int chunkPosition = WorldGenerator.instance.GetChunkPosition(GetComponent<Building>().gridPosition);
        parentChunk = WorldGenerator.instance.TryGetChunk(chunkPosition);
    }
}
