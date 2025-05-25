using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public Chunk parentChunk;
    public Building building;

    
    private void Start()
    {
        building = gameObject.GetComponent<Building>();
        building.gettingRemoved += RemovingResource;

        GameManager.instance.resources.Add(gameObject);
        Vector2Int chunkPosition = WorldGenerator.instance.GetChunkPosition(GetComponent<Building>().gridPosition);
        parentChunk = WorldGenerator.instance.TryGetChunk(chunkPosition);
    }
    
    
    public void RemovingResource(Building building)
    {
        GameManager.instance.resources.Remove(gameObject);
        Destroy(gameObject);
    }
}
