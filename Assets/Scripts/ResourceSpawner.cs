using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    public GameObject SpawnResources(Vector2Int position, BiomeData biome)
    {
        // Find the biome data
        if (biome == null)
        {
            Debug.LogError($"Biome not found!");
            return null;
        }

        // Select a resource based on weight
        if (Random.Range(0f, 1f) < 0.2f) // Adjust the probability as needed
        {
            // Select a resource based on weight
            GameObject resourcePrefab = GetRandomResource(biome.resources);
            if (resourcePrefab != null)
            {
                // Spawn the resource at the specified position
                GameObject resource =  Instantiate(resourcePrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
                return resource;
            }
        }
        return null;
    }

    // Get a random resource based on weight
    private GameObject GetRandomResource(List<BiomeResource> resources)
    {
        // Calculate the total weight
        int totalWeight = 0;
        foreach (var resource in resources)
        {
            totalWeight += resource.weight;
        }

        // Generate a random number between 0 and totalWeight
        int randomValue = Random.Range(0, totalWeight);

        // Select the resource based on the random value
        foreach (var resource in resources)
        {
            if (randomValue < resource.weight)
            {
                return resource.prefab;
            }
            randomValue -= resource.weight;
        }

        return null; // No resource selected
    }
}