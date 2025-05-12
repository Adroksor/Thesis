using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[System.Serializable]
[CreateAssetMenu(fileName = "NewBiomeData", menuName = "Biome Data")]
public class BiomeData : ScriptableObject
{
    public string biomeName; // Name of the biome (e.g., Forest, Desert)
    
    [Header("Perlin Noise Settings")]
    public float baseScale = 10f;   // Base zoom level
    public int octaves = 4;         // Number of noise layers
    public float persistence = 0.5f; // Controls amplitude decrease per octave
    public float lacunarity = 2.0f;  // Controls frequency increase per octave
    [Range(0f, 1f)]
    public float waterLevel;
    
    [Header("Resources Settings")]
    [Range(0f, 1f)]
    public float resourceSpawnRate;
    public List<BiomeResource> resources; // List of resources for this biome
    public List<TileBase> biomeTiles;
    public TileBase waterTile;
}
