using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class WorldGenerator : MonoBehaviour
{
    public int chunkSize = 16; // Size of each chunk (e.g., 16x16 tiles)
    public int renderDistance = 3; // Number of chunks to load around the player
    public Tilemap tilemap; // Reference to the Tilemap component
    public TileBase waterTile; // Tile for water
    public float altitudeScale = 10f; // Scale for altitude noise

    public List<BiomeData> biomeList;
    public ResourceSpawner resourceSpawner;
    
    [Header("Noise Settings")]
    public float scale = 20f; // Scale of the Perlin noise
    public int octaves = 4; // Number of noise layers (octaves)
    public float persistence = 0.5f; // Amplitude multiplier for each octave
    public float lacunarity = 2f; // Frequency multiplier for each octave
    public Vector2 noiseOffset; // Random offset for the noise

    private Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>(); // Dictionary to store all chunks
    private Vector2Int playerChunkPosition; // Current chunk position of the player

    private Transform player; // Reference to the player's transform
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player object

        // Generate a random offset for the noise
        noiseOffset = new Vector2(Random.Range(100f, 1000f), Random.Range(100f, 1000f));

        UpdateChunks();
    }

    void Update()
    {
        // Check if the player has moved to a new chunk
        Vector2Int currentPlayerChunkPosition = GetChunkPosition(player.position);
        if (currentPlayerChunkPosition != playerChunkPosition)
        {
            playerChunkPosition = currentPlayerChunkPosition;
            UpdateChunks();
        }

    }

    void UpdateChunks()
    {
        // Load chunks based on the player's position
        for (int x = playerChunkPosition.x - renderDistance; x <= playerChunkPosition.x + renderDistance; x++)
        {
            for (int y = playerChunkPosition.y - renderDistance; y <= playerChunkPosition.y + renderDistance; y++)
            {
                Vector2Int chunkPosition = new Vector2Int(x, y);

                if (!chunks.ContainsKey(chunkPosition))
                {
                    // Generate and load the chunk if it doesn't exist
                    Chunk chunk = GenerateChunk(chunkPosition);
                    chunks[chunkPosition] = chunk;
                    LoadChunk(chunk);
                }
                else if (!chunks[chunkPosition].isLoaded)
                {
                    // Load the chunk if it exists but isn't loaded
                    LoadChunk(chunks[chunkPosition]);
                }
            }
        }
    }

    Chunk GenerateChunk(Vector2Int chunkPosition)
    {
        Chunk chunk = new Chunk(chunkPosition, chunkSize);

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                // Calculate the world position of the tile
                int worldX = chunkPosition.x * chunkSize + x;
                int worldY = chunkPosition.y * chunkSize + y;

                // Generate multi-octave Perlin noise for altitude
                float altitude = CalculateMultiOctaveNoise(worldX, worldY, scale, octaves, persistence, lacunarity, noiseOffset) * altitudeScale;

                string biomeName = DetermineBiome(worldX, worldY);
                
                BiomeData biome = biomeList.Find(b => b.biomeName == biomeName);

                if (biome == null)
                {
                    Debug.Log("Biome not found: " + biomeName);
                    return null;
                }
                
                // Determine biome based on altitude, temperature, and moisture
                TileBase tile = GetTileForBiome(altitude, biome);
                
                // Spawn resources for this biome
                if(tile != null && tile != waterTile)
                {
                    GameObject resource = resourceSpawner.SpawnResources(new Vector2Int(worldX, worldY), biome);
                
                    resource.transform.parent = chunk.chunkOBJ.transform;
                }

                // Store the tile in the chunk
                chunk.tiles[x, y] = tile;
            }
        }

        // Post-process the chunk to fill in single isolated cells
        PostProcessChunk(chunk);
        chunk.isLoaded = true;
        return chunk;
    }

    void PostProcessChunk(Chunk chunk)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                // Check if the current tile is isolated
                if (IsTileIsolated(chunk, x, y))
                {
                    // Replace the isolated tile with the most common surrounding tile
                    chunk.tiles[x, y] = GetMostCommonSurroundingTile(chunk, x, y);
                }
            }
        }
    }
    
    private string DetermineBiome(int worldX, int worldY)
    {
        // Example: Use Perlin noise to determine the biome
        float temperature = Mathf.PerlinNoise((float)(worldX + noiseOffset.x) / (chunkSize * scale * 2), (float)(worldY + noiseOffset.y) / (chunkSize * scale * 2));
        float moisture = Mathf.PerlinNoise((float)(worldX + noiseOffset.x) / (chunkSize * scale * 2), (float)(worldY + noiseOffset.y) / (chunkSize * scale * 2));

        if (temperature < 0.3f)
        {
            return "SnowBiome";
        }
        else if (temperature < 0.6f)
        {
            //if (moisture < 0.6f) 

            return "ForestBiome";
            //else if (moisture < 0.6f) return "Forest";
            //else return "Swamp";
        }
        else
        {
            //if (moisture < 0.3f) 
                return "DesertBiome";
            //else if (moisture < 0.6f) return "Savanna";
            //else return "Rainforest";
        }
    }

    bool IsTileIsolated(Chunk chunk, int x, int y)
    {
        // Get the current tile
        TileBase currentTile = chunk.tiles[x, y];

        // Check all 8 neighboring tiles
        int isolatedCount = 0;
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue; // Skip the current tile

                int nx = x + dx;
                int ny = y + dy;

                // Check if the neighbor is within bounds
                if (nx >= 0 && nx < chunkSize && ny >= 0 && ny < chunkSize)
                {
                    if (chunk.tiles[nx, ny] == currentTile)
                    {
                        isolatedCount++;
                    }
                }
            }
        }

        // If the tile has no neighbors of the same type, it's isolated
        return isolatedCount == 0;
    }

    TileBase GetMostCommonSurroundingTile(Chunk chunk, int x, int y)
    {
        // Count the frequency of each surrounding tile type
        Dictionary<TileBase, int> tileCounts = new Dictionary<TileBase, int>();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue; // Skip the current tile

                int nx = x + dx;
                int ny = y + dy;

                // Check if the neighbor is within bounds
                if (nx >= 0 && nx < chunkSize && ny >= 0 && ny < chunkSize)
                {
                    TileBase tile = chunk.tiles[nx, ny];
                    if (tileCounts.TryAdd(tile, 1))
                    {
                        tileCounts[tile]++;
                    }
                    else
                    {
                        tileCounts[tile] = 1;
                    }
                }
            }
        }

        // Find the most common tile
        TileBase mostCommonTile = null;
        int maxCount = 0;
        foreach (var pair in tileCounts)
        {
            if (pair.Value > maxCount)
            {
                mostCommonTile = pair.Key;
                maxCount = pair.Value;
            }
        }

        return mostCommonTile;
    }


    float CalculateMultiOctaveNoise(float x, float y, float scale, int octaves, float persistence, float lacunarity, Vector2 offset)
    {
        float amplitude = 1f;
        float frequency = 1f;
        float noiseHeight = 0f;

        for (int i = 0; i < octaves; i++)
        {
            float sampleX = (x + offset.x) / scale * frequency;
            float sampleY = (y + offset.y) / scale * frequency;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1; // Range [-1, 1]
            noiseHeight += perlinValue * amplitude;

            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return noiseHeight;
    }

    TileBase GetTileForBiome(float altitude, BiomeData biomeData)
    {
        if (altitude < biomeData.waterLevel)
        {
            return waterTile;
        }
        return biomeData.biomeTiles[0];
    }

    void LoadChunk(Chunk chunk)
    {
        chunk.isLoaded = true;
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                // Calculate the world position of the tile
                int worldX = chunk.position.x * chunkSize + x;
                int worldY = chunk.position.y * chunkSize + y;

                // Set the tile in the tilemap
                Vector3Int tilePosition = new Vector3Int(worldX, worldY, 0);
                tilemap.SetTile(tilePosition, chunk.tiles[x, y]);
            }
        }        
    }

    void UnloadChunk(Chunk chunk)
    {
        chunk.isLoaded = false;
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                // Calculate the world position of the tile
                int worldX = chunk.position.x * chunkSize + x;
                int worldY = chunk.position.y * chunkSize + y;

                // Remove the tile from the tilemap
                Vector3Int tilePosition = new Vector3Int(worldX, worldY, 0);
                tilemap.SetTile(tilePosition, null);
            }
        }
    }

    Vector2Int GetChunkPosition(Vector3 worldPosition)
    {
        // Convert world position to chunk position
        int chunkX = Mathf.FloorToInt(worldPosition.x / chunkSize);
        int chunkY = Mathf.FloorToInt(worldPosition.y / chunkSize);
        return new Vector2Int(chunkX, chunkY);
    }
    
    public void ResetWorld()
    {
        // Clear all loaded chunks
        foreach (var chunk in chunks.Values)
        {
            UnloadChunk(chunk);
            Destroy(chunk.chunkOBJ);
        }
        chunks.Clear();

        // Generate a new random offset for the noise
        noiseOffset = new Vector2(Random.Range(0f, 1000f), Random.Range(0f, 1000f));

        // Regenerate the world around the player
        UpdateChunks();
    }
    private void OnDrawGizmos()
    {
        if (chunks == null) return;

        // Set the color for the chunk edges
        Gizmos.color = Color.red;

        // Draw lines for each chunk
        foreach (var chunk in chunks.Values)
        {
            // Calculate the world position of the chunk's bottom-left corner
            Vector3 chunkStart = new Vector3(
                chunk.position.x * chunkSize - 0.5f,
                chunk.position.y * chunkSize - 0.5f,
                0
            );

            // Draw the chunk boundaries
            Vector3 chunkEndX = chunkStart + new Vector3(chunkSize, 0, 0);
            Vector3 chunkEndY = chunkStart + new Vector3(0, chunkSize, 0);
            Vector3 chunkEndXY = chunkStart + new Vector3(chunkSize, chunkSize, 0);

            // Draw the lines
            Gizmos.DrawLine(chunkStart, chunkEndX); // Bottom edge
            Gizmos.DrawLine(chunkStart, chunkEndY); // Left edge
            Gizmos.DrawLine(chunkEndX, chunkEndXY); // Top edge
            Gizmos.DrawLine(chunkEndY, chunkEndXY); // Right edge
        }
    }
}

[System.Serializable]
public class BiomeResource
{
    public GameObject prefab;
    public int weight;
}
