using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ResourceSpawner : MonoBehaviour
{

    [Range(0, 1)]
    public List<float> spawnRate;
    
    
    public GameObject GetDeterministicResource(
        List<BiomeResource> resources,
        int worldX, int worldY, int seed,
        float epicChance, float rareChance, float commonChance)  // <-- dodany parametr
    {
        if (resources == null || resources.Count == 0)
            return null;

        var common = new List<BiomeResource>();
        var rare   = new List<BiomeResource>();
        var epic   = new List<BiomeResource>();

        foreach (var r in resources)
            (r.rarity switch
            { ResourceRarity.Epic => epic,
                ResourceRarity.Rare => rare,
                _                   => common }).Add(r);

        float roll = Hash01(worldX, worldY, seed);  // 0-1

        if (roll < epicChance && epic.Count > 0)
            return WeightedPick(epic, worldX, worldY, seed);

        roll -= epicChance;
        if (roll < rareChance && rare.Count > 0)
            return WeightedPick(rare, worldX, worldY, seed ^ 1337);

        roll -= rareChance;
        if (roll < commonChance && common.Count > 0)
            return WeightedPick(common, worldX, worldY, seed ^ 4242);

        return null;
    }

    
    GameObject WeightedPick(List<BiomeResource> list, int x, int y, int seed)
    {
        int total = 0;
        foreach (var r in list) total += r.weight;

        if (total == 0) return null;

        int pick = Hash(x ^ 17, y ^ 31, seed) % total;
        foreach (var r in list)
        {
            if (pick < r.weight) return r.prefab;
            pick -= r.weight;
        }
        return null;
    }


    public bool IsTileNearWater(Vector2 position, int distance)
    {
        Tilemap waterMap   = WorldGenerator.instance.waterLevel;

        int baseX = Mathf.FloorToInt(position.x);
        int baseY = Mathf.FloorToInt(position.y);

        for (int dx = -distance; dx <= distance; ++dx)
        {
            for (int dy = -distance; dy <= distance; ++dy)
            {
                if (dx == 0 && dy == 0) continue;

                Vector3Int checkPos = new Vector3Int(baseX + dx, baseY + dy, 0);

                TileBase tile = waterMap.GetTile(checkPos);
                if (tile != null)
                {
                    return true;
                }
            }
        }

        // w promieniu ‹distance› nie znaleziono wody
        return false;
    }
    
    // Helpers you can paste in ResourceSpawner (or a Utility class)
    static int Hash(int x, int y, int seed)
    {
        unchecked
        {
            int h = x * 73856093 ^ y * 19349663 ^ seed;
            h = (h ^ 0x27d4eb2d) * 0x165667b1;
            h ^= h >> 15;
            return h & 0x7fffffff;              // keep it positive
        }
    }

    static float Hash01(int x, int y, int seed) =>
        Hash(x, y, seed) / 2147483647f;         // map to [0,1]
}

public enum ResourceRarity { Common, Uncommon, Rare, Epic }

[System.Serializable]
public class BiomeResource
{
    public GameObject prefab;
    public ResourceRarity rarity;
    public int weight;
}

