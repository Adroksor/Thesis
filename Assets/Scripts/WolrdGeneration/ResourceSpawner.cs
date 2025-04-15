using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{

    [Range(0, 1)]
    public List<float> spawnRate;
    public GameObject SpawnResources(Vector2Int position, BiomeData biome)
    {
        // Find the biome data
        if (biome == null)
        {
            Debug.LogError($"Biome not found!");
            return null;
        }

        // Select a resource based on weight
        if (Random.Range(0f, 1f) < biome.resourceSpawnRate) // Adjust the probability as needed
        {
            // Select a resource based on weight
            GameObject resourcePrefab = GetRandomResource(biome.resources);
            if (resourcePrefab != null)
            {
                // Spawn the resource at the specified position
                GameObject resource = Instantiate(resourcePrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
                return resource;
            }
        }
        return null;
    }

    // Get a random resource based on weight
    private GameObject GetRandomResource(List<BiomeResource> resources)
    {
        if (resources == null || resources.Count == 0)
        {
            Debug.LogWarning("No resources available in this biome.");
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
}

public enum ResourceRarity { Common, Rare, Epic }

[System.Serializable]
public class BiomeResource
{
    public GameObject prefab;
    public ResourceRarity rarity;
    public int weight;
}
