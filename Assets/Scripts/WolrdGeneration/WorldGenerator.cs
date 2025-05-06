using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class WorldGenerator : MonoBehaviour
{
    public int seed;
    public int chunkSize = 16; // Size of each chunk (e.g., 16x16 tiles)
    public GameObject chunkList;
    public int renderDistance = 3; // Number of chunks to load around the player
    public Tilemap groundLevel;
    public Tilemap waterLevel;
    public TileBase waterTile; // Tile for water

    public List<BiomeData> biomeList;
    public ResourceSpawner resourceSpawner;
    
    [Header("Noise Settings")]
    public float scale = 20f; // Scale of the Perlin noise
    public int octaves = 4; // Number of noise layers (octaves)
    public float persistence = 0.5f; // Amplitude multiplier for each octave
    public float lacunarity = 2f; // Frequency multiplier for each octave
    public Vector2 noiseOffset;
    public int safeRadius = 256;
    public float maxUplift = 1;

    public Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>(); // Dictionary to store all chunks
    public List<Chunk> LoadedChunks = new List<Chunk>();
    [SerializeField]
    private Vector2Int playerChunkPosition; // Current chunk position of the player

    private Transform player; // Reference to the player's transform

    public bool spawnResources = false;
    public static WorldGenerator instance { get; private set; }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Multiple WorldGenerator instances found!");
            Destroy(gameObject);
        }
    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player object

        // Generate a random offset for the noise
        seed = GameManager.instance.seed;
        int randomX = seed;
        int randomY = seed / 2;
        noiseOffset = new Vector2(randomX, randomY);

        UpdateChunks();
    }

    void Update()
    {
        
        Vector2Int currentPlayerChunkPosition = GetChunkPosition(player.position);
        if (currentPlayerChunkPosition != playerChunkPosition)
        {
            playerChunkPosition = currentPlayerChunkPosition;
            UpdateChunks();

            //foreach (var chunk in chunks)
            //{
            //    Debug.Log(chunk.Key);
            //}
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
        // After loading/generating all visible chunks
        for (int x = playerChunkPosition.x - (renderDistance - 1); x <= playerChunkPosition.x + (renderDistance - 1); x++)
        {
            for (int y = playerChunkPosition.y - (renderDistance - 1); y <= playerChunkPosition.y + (renderDistance - 1); y++)
            {
                Vector2Int chunkPosition = new Vector2Int(x, y);
                if (chunks.TryGetValue(chunkPosition, out var chunk) && !chunk.resourcesSpawned)
                {
                    SpawnResourcesForChunk(chunk);
                    chunk.resourcesSpawned = true;
                }
            }
        }

        
        
        UnloadDistantChunks();
    }
    
    void UnloadDistantChunks()
    {
        List<Vector2Int> chunksToUnload = new List<Vector2Int>();

        // Find chunks that are outside the render distance and add them to the unload list
        foreach (var chunk in chunks)
        {
            if(!chunk.Value.isLoaded)
                continue;
            Vector2Int chunkPosition = chunk.Key;
            int distance = Mathf.Abs(chunkPosition.x - playerChunkPosition.x) + Mathf.Abs(chunkPosition.y - playerChunkPosition.y);
        
            // If chunk is too far, mark it for unloading
            if (distance > renderDistance)
            {
                chunksToUnload.Add(chunkPosition);
            }
        }

        // Unload the distant chunks
        foreach (var chunkPosition in chunksToUnload)
        {
            Chunk chunk = chunks[chunkPosition];
            UnloadChunk(chunk);
        }
    }
    
    void SpawnResourcesForChunk(Chunk chunk)
    {
        Vector2Int chunkPosition = chunk.position;

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                TileBase tile = chunk.tiles[x, y];

                if (tile == null || tile == waterTile)
                    continue;

                int worldX = chunkPosition.x * chunkSize + x;
                int worldY = chunkPosition.y * chunkSize + y;

                string biomeName = DetermineBiome(worldX, worldY);
                BiomeData biome = biomeList.Find(b => b.biomeName == biomeName);
                if (biome == null)
                    continue;

                GameObject resourcePrefab = resourceSpawner.GetRandomResource(biome.resources);
                if (resourcePrefab == null)
                    continue;

                try
                {
                    Building building = resourcePrefab.GetComponent<Building>();
                    if (building != null)
                    {
                        if (BuildingGrid.instance.CanPlace(new Vector2Int(worldX, worldY), building))
                        {
                            GameObject resource = Instantiate(resourcePrefab, new Vector3(worldX, worldY, 0), Quaternion.identity);
                            resource.name = resourcePrefab.name;
                            Building resBuilding = resource.GetComponent<Building>();
                            resBuilding.Place(new Vector2Int(worldX, worldY));
                            resource.transform.parent = chunk.chunkOBJ.transform;
                        }
                    }
                }
                catch (System.Exception)
                {
                    // Silently skip any errors during placement
                }
            }
        }
    }


    Chunk GenerateChunk(Vector2Int chunkPosition)
    {
        Chunk chunk = new Chunk(chunkPosition, chunkSize, chunkList);
        chunks.Add(chunkPosition, chunk);
        
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                // Calculate the world position of the tile
                int worldX = chunkPosition.x * chunkSize + x;
                int worldY = chunkPosition.y * chunkSize + y;
                string biomeName = DetermineBiome(worldX, worldY);
                
                BiomeData biome = biomeList.Find(b => b.biomeName == biomeName);

                if (biome == null)
                {
                    Debug.Log("Biome not found: " + biomeName);
                    return null;
                }
                
                // Generate multi-octave Perlin noise for altitude
                float altitude = CalculateMultiOctaveNoise(new Vector2(worldX, worldY), biome, noiseOffset);
                
                float dist = Vector2.Distance(new Vector2(worldX, worldY),
                    Vector2.zero);
                if (dist < safeRadius)
                {
                    // 0 → 1 fall‑off curve (1 at centre, 0 at edge)
                    float t = 1f - (dist / safeRadius);
                    float uplift = maxUplift * Mathf.SmoothStep(0f, 1f, t);   // smooth fade
                    altitude += uplift;
                }
                
                // Determine biome based on altitude, temperature, and moisture
                TileBase tile = GetTileForBiome(altitude, biome);

                // Store the tile in the chunk
                chunk.tiles[x, y] = tile;
            }
        }

        // Post-process the chunk to fill in single isolated cells
        PostProcessChunk(chunk);
        chunk.isLoaded = true;
        if(!LoadedChunks.Contains(chunk))
            LoadedChunks.Add(chunk);
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
        float temperature = CalculateMultiOctaveNoise(
            new Vector2(worldX + noiseOffset.x, worldY + noiseOffset.y), 
            scale, 
            octaves, // Number of octaves
            persistence, // Persistence (controls amplitude drop per octave)
            lacunarity // Lacunarity (controls frequency increase per octave)
        );

        Vector2 moistureOffset = new Vector2(noiseOffset.x + 500f, noiseOffset.y + 500f);
        float moisture = CalculateMultiOctaveNoise(
            new Vector2(worldX + moistureOffset.x, worldY + moistureOffset.y), 
            scale, 
            octaves, // Number of octaves
            persistence, // Persistence (controls amplitude drop per octave)
            lacunarity // Lacunarity (controls frequency increase per octave)
        );

        if (temperature < 0.3f)
        {
            return "Snow";
        }
        
        if (temperature < 0.6f)
        {
            if (moisture < 0.3f) 
                return "Ocean";
            if (moisture < 0.6f) 
                return "Forest";
            return "Swamp";
        }
        
        else
        {
            if (moisture < 0.3f) 
                return "Desert";
            if (moisture < 0.6f) 
                return "Desert"; //"Savanna"
            return "Desert"; //"Rainforest"
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
                    if (tileCounts.ContainsKey(tile))
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
    
    
    public static float CalculateMultiOctaveNoise(Vector2 position, BiomeData biome, Vector2 offset)
    {
        float total = 0f;
        float frequency = 1f;
        float amplitude = 1f;
        float maxValue = 0f;

        for (int i = 0; i < biome.octaves; i++)
        {
            float noiseValue = Mathf.PerlinNoise(
                (position.x + offset.x) / (biome.baseScale * frequency), 
                (position.y + offset.y) / (biome.baseScale * frequency)
            );

            total += noiseValue * amplitude;
            maxValue += amplitude;

            amplitude *= biome.persistence;
            frequency *= biome.lacunarity;
        }
        return total / maxValue; // Normalize to [0,1]
    }
    
    float CalculateMultiOctaveNoise(Vector2 position, float scale, int octaves, float persistence, float lacunarity)
    {
        float total = 0f;
        float frequency = 1f;
        float amplitude = 1f;
        float maxValue = 0f;

        for (int i = 0; i < octaves; i++)
        {
            float noiseValue = Mathf.PerlinNoise(
                (position.x + noiseOffset.x) / (scale * frequency), 
                (position.y + noiseOffset.y) / (scale * frequency)
            );

            total += noiseValue * amplitude;
            maxValue += amplitude;

            amplitude *= persistence;
            frequency *= lacunarity;
        }
        return total / maxValue; // Normalize to [0,1]
    }
    


    TileBase GetTileForBiome(float altitude, BiomeData biomeData)
    {
        if (altitude < biomeData.waterLevel)
        {
            return waterTile;
        }
        return biomeData.biomeTiles[0];
    }

    TileBase GetTileAtPosition(Vector2Int position)
    {
        return groundLevel.GetTile(new Vector3Int(position.x, position.y, 0));
    }
    

    void LoadChunk(Chunk chunk)
    {
        chunk.chunkOBJ.SetActive(true);
        if(!LoadedChunks.Contains(chunk))
            LoadedChunks.Add(chunk);
        chunk.isLoaded = true;
        if (!chunk.tilesSpawned)
        {   
            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    // Calculate the world position of the tile
                    int worldX = chunk.position.x * chunkSize + x;
                    int worldY = chunk.position.y * chunkSize + y;

                    // Set the tile in the tilemap
                    Vector3Int tilePosition = new Vector3Int(worldX, worldY, 0);
                    if (chunk.tiles[x, y].name == "WaterTile")
                        waterLevel.SetTile(tilePosition, chunk.tiles[x, y]);
                    else
                        groundLevel.SetTile(tilePosition, chunk.tiles[x, y]);
                }
            }        
            chunk.tilesSpawned = true;
        }
    }

    void UnloadChunk(Chunk chunk)
    {
        chunk.chunkOBJ.SetActive(false);
        chunk.isLoaded = false;
        LoadedChunks.Remove(chunk);
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
        
        seed = GameManager.instance.seed;
        int randomX = seed;
        int randomY = seed / 2;
        noiseOffset = new Vector2(randomX, randomY);


        // Regenerate the world around the player
        UpdateChunks();
    }
    
    public Chunk TryGetChunk(Vector2Int chunkPosition)
    {
        chunks.TryGetValue(chunkPosition, out var chunk);
        return chunk;
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
