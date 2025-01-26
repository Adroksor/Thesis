using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[System.Serializable]
[CreateAssetMenu(fileName = "NewBiomeData", menuName = "Biome Data")]
public class BiomeData : ScriptableObject
{
    public string biomeName; // Name of the biome (e.g., Forest, Desert)
    public float waterLevel;
    public List<BiomeResource> resources; // List of resources for this biome
    public List<TileBase> biomeTiles;
    
}
