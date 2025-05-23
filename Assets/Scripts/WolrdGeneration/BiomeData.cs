using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[System.Serializable]
[CreateAssetMenu(fileName = "NewBiomeData", menuName = "Biome Data")]
public class BiomeData : ScriptableObject
{
    public string biomeName;

    [Header("Noise Settings")]
    [Header("Macro")]

    public float macroBaseScale   = 400f;
    public int   macroOctaves     = 3;
    public float macroPersistence = 0.5f;
    public float macroLacunarity  = 2.0f;
                 
    [Header("Micro")]
    public float microBaseScale   = 40f;
    public int   microOctaves     = 5;
    public float microPersistence = 0.45f;
    public float microLacunarity  = 2.3f;
    public float warpStrength     = 10f;
    
    [Header("Lake Noise Settings")]
    public float lakeScale       = 30f;
    public int   lakeOctaves     = 3;
    public float lakePersistence = 0.5f;
    public float lakeLacunarity  = 2f;
// Threshold decyduje, od jakiego poziomu szumu zaczyna się woda
    [Range(0f,1f)] public float lakeThreshold = 0.65f;
    
    [Range(0f, 1f)]
    public float waterLevel;
    
    [Header("Resources Settings")]
    public List<BiomeResource> resources;
    public List<TileBase> biomeTiles;
    public TileBase waterTile;
}
