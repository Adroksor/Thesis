using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{

    [Range(0, 1)]
    public List<float> spawnRate;

    // Get a random resource based on weight
    public GameObject GetRandomResource(List<BiomeResource> resources)
    {
        if (resources == null || resources.Count == 0)
        {
            return null;
        }

        // Separate resources by rarity
        var common = new List<BiomeResource>();
        var rare = new List<BiomeResource>();
        var epic = new List<BiomeResource>();

        foreach (var res in resources)
        {
            switch (res.rarity)
            {
                case ResourceRarity.Common:
                    common.Add(res);
                    break;
                case ResourceRarity.Rare:
                    rare.Add(res);
                    break;
                case ResourceRarity.Epic:
                    epic.Add(res);
                    break;
            }
        }

        // Set probabilities for each tier (adjust as needed)
        //float commonChance = spawnRate[0];
        float rareChance = spawnRate[1];
        float epicChance = spawnRate[2];

        float roll = Random.value;

        List<BiomeResource> chosenList;
        if (roll < epicChance && epic.Count > 0) chosenList = epic;
        else if (roll < epicChance + rareChance && rare.Count > 0) chosenList = rare;
        else chosenList = common;

        return GetRandomResourceFromList(chosenList);
    }
    
    public GameObject GetDeterministicResource(
        List<BiomeResource> resources,
        int worldX, int worldY, int seed,
        float epicChance, float rareChance)
    {
        if (resources == null || resources.Count == 0)
            return null;

        // Split by rarity once (could cache per-biome)
        var common = new List<BiomeResource>();
        var rare   = new List<BiomeResource>();
        var epic   = new List<BiomeResource>();
        foreach (var r in resources)
            (r.rarity switch
            { ResourceRarity.Epic   => epic,
                ResourceRarity.Rare   => rare,
                _                     => common }).Add(r);

        float roll = Hash01(worldX, worldY, seed);

        List<BiomeResource> chosen;
        if (roll < epicChance && epic.Count > 0)           chosen = epic;
        else if (roll < epicChance + rareChance && rare.Count > 0) chosen = rare;
        else                                                 chosen = common;

        if (chosen.Count == 0)
            return null;
        
        // weighted pick, but deterministic
        int total = 0;
        foreach (var r in chosen) total += r.weight;
        int pick = Hash(worldX ^ 17, worldY ^ 31, seed) % total;   // second hash

        foreach (var r in chosen)
        {
            if (pick < r.weight) return r.prefab;
            pick -= r.weight;
        }
        return null;
    }


    private GameObject GetRandomResourceFromList(List<BiomeResource> list)
    {
        if (list == null || list.Count == 0) return null;

        int totalWeight = 0;
        foreach (var res in list)
        {
            totalWeight += res.weight;
        }

        int randomValue = Random.Range(0, totalWeight);

        foreach (var res in list)
        {
            if (randomValue < res.weight)
            {
                return res.prefab;
            }
            randomValue -= res.weight;
        }

        return null;
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

public enum ResourceRarity { Common, Rare, Epic }

[System.Serializable]
public class BiomeResource
{
    public GameObject prefab;
    public ResourceRarity rarity;
    public int weight;
}

