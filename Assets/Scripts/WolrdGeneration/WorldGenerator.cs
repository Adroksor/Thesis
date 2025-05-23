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
    private Dictionary<TileBase, BiomeData> tileToBiome;

    
    public LaunchMode launchMode;
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
        Debug.Log(Application.persistentDataPath);

    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player object
        GenerateTileToBiomeDictionary();
    }

    void Update()
    {
        Vector2Int currentPlayerChunkPosition = GetChunkPosition(player.position);
        if (currentPlayerChunkPosition != playerChunkPosition)
        {
            playerChunkPosition = currentPlayerChunkPosition;
            UpdateChunks();
        }
    }
    
    public void SetSeed(int s)
    {
        seed = s;
        noiseOffset = new Vector2(seed, (float)seed / 2);
    }

    public void GenerateInitialChunks()
    {
        chunks.Clear();
        UpdateChunks();
    }


    void UpdateChunks()
    {
        for (int x = playerChunkPosition.x - renderDistance; x <= playerChunkPosition.x + renderDistance; x++)
        {
            for (int y = playerChunkPosition.y - renderDistance; y <= playerChunkPosition.y + renderDistance; y++)
            {
                Vector2Int chunkPosition = new Vector2Int(x, y);

                if (!chunks.ContainsKey(chunkPosition))
                {
                    Chunk chunk = GenerateChunk(chunkPosition);
                    chunks[chunkPosition] = chunk;
                    LoadChunk(chunk);
                }
                else if (!chunks[chunkPosition].isLoaded)
                {
                    LoadChunk(chunks[chunkPosition]);
                }
            }
        }
        for (int x = playerChunkPosition.x - (renderDistance - 1); x <= playerChunkPosition.x + (renderDistance - 1); x++)
        {
            for (int y = playerChunkPosition.y - (renderDistance - 1); y <= playerChunkPosition.y + (renderDistance - 1); y++)
            {
                Vector2Int chunkPosition = new Vector2Int(x, y);
                if (chunks.TryGetValue(chunkPosition, out var chunk) && !chunk.resourcesSpawned)
                {
                    if (spawnResources)
                    {
                        SpawnResourcesForChunk(chunk);
                    }
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
                UnloadChunk(chunk.Value);
            }
        }
    }
    
    public void SpawnResourcesForChunk(Chunk chunk)
    {
        Vector2Int chunkPosition = chunk.position;

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                TileBase tile = chunk.tiles[x, y];
                if (tile == null) continue;
                if (!tileToBiome.TryGetValue(tile, out BiomeData biome))
                    continue;
                
                int worldX = chunkPosition.x * chunkSize + x;
                int worldY = chunkPosition.y * chunkSize + y;
                
                if (biome == null)
                    continue;
                if (tile == biome.waterTile) continue;

                GameObject resourcePrefab = resourceSpawner.GetDeterministicResource(
                    biome.resources,
                    worldX, worldY, seed,
                    resourceSpawner.spawnRate[2],   // epicChance
                    resourceSpawner.spawnRate[1],   // epicChance
                    resourceSpawner.spawnRate[0]);  // rareChance
                if (resourcePrefab == null)
                    continue;
                if (resourcePrefab.name == "Reeds")
                    if(!resourceSpawner.IsTileNearWater(new Vector2(worldX, worldY), 2))
                    {
                        continue;
                    }
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
                        GameManager.instance.resources.Add(resource);
                    }
                }
            }
        }
        chunk.resourcesSpawned = true;
    }


   
    public Chunk GenerateChunk(Vector2Int chunkPos)
    {
        Chunk chunk = new Chunk(chunkPos, chunkSize, chunkList);
        chunks.Add(chunkPos, chunk);

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                {
                    int worldX = chunkPos.x * chunkSize + x + (int)noiseOffset.x;
                    int worldY = chunkPos.y * chunkSize + y + (int)noiseOffset.y;
                    
                    Vector2 pos = new Vector2(worldX, worldY);
                    
                    string biomeName = DetermineBiome(pos);
                
                    BiomeData biome = biomeList.Find(b => b.biomeName == biomeName);

                    if (biome == null)
                    {
                        Debug.Log("Biome not found: " + biomeName);
                        return null;
                    }
                    
                    float heightMacro = FractalNoise(
                        pos,
                        biome.macroBaseScale,
                        biome.macroOctaves,
                        biome.macroPersistence,
                        biome.macroLacunarity);

                    float heightMicro = WarpedFBM(
                        pos,
                        biome.microBaseScale,
                        biome.microOctaves,
                        biome.microPersistence,
                        biome.microLacunarity,
                        biome.warpStrength);
                    float totalHeight = Mathf.Lerp(heightMacro, heightMicro, 0.35f);
                    
                    TileBase tile = GetTileForBiome(totalHeight, biome, worldX, worldY);

                    chunk.tiles[x, y] = tile;
                }
            }
        }
        PostProcessChunk(chunk);

        chunk.isLoaded = true;
        if (!LoadedChunks.Contains(chunk)) LoadedChunks.Add(chunk);
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
    
    private string DetermineBiome(Vector2 pos)
    {
        float temperature    = FractalNoise(
            pos+noiseOffset, scale, octaves, persistence, lacunarity);
        float moisture   = FractalNoise(
            pos+noiseOffset * 2, scale, octaves, persistence,  lacunarity);
        if (temperature < 0.33f)
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
            if (moisture < 0.33f) 
                return "Desert";
            if (moisture < 0.7f) 
                return "Desert";
            return "Desert";
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
    
    float FractalNoise(Vector2 pos,
        float   scale,
        int     octaves,
        float   persistence,
        float   lacunarity)
    {
        float value     = 0f;
        float amplitude = 1f;
        float frequency = 1f;
        float norm      = 0f;

        for (int i = 0; i < octaves; i++)
        {
            float n = Mathf.PerlinNoise(
                (pos.x) / (scale / frequency),
                (pos.y) / (scale / frequency));

            value     += n * amplitude;
            norm      += amplitude;
            amplitude *= persistence;
            frequency *= lacunarity;
        }
        return value / norm;
    }
    
    float WarpedFBM(Vector2 pos,
        float   scale,
        int     octaves,
        float   persistence,
        float   lacunarity,
        float   warpStrength)
    {
        float wx = Mathf.PerlinNoise(pos.x * 0.01f, pos.y * 0.01f);
        float wy = Mathf.PerlinNoise((pos.x + 1234) * 0.01f, (pos.y + 5678) * 0.01f);
        
        Vector2 warped = pos + new Vector2(wx, wy) * warpStrength;

        return FractalNoise(warped, scale, octaves, persistence, lacunarity);
    }
    
    public static float CalculateMultiOctaveNoise(Vector2 position, BiomeData biome, Vector2 offset)
    {
        float total = 0f;
        float frequency = 1f;
        float amplitude = 1f;
        float maxValue = 0f;

        for (int i = 0; i < biome.macroOctaves; i++)
        {
            float noiseValue = Mathf.PerlinNoise(
                (position.x + offset.x) / (biome.macroBaseScale * frequency), 
                (position.y + offset.y) / (biome.macroBaseScale * frequency)
            );

            total += noiseValue * amplitude;
            maxValue += amplitude;

            amplitude *= biome.macroPersistence;
            frequency *= biome.macroLacunarity;
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
    


    
    TileBase GetTileForBiome(float altitude, BiomeData biome, int worldX, int worldY)
    {
        if (altitude < biome.waterLevel)
            return biome.waterTile;

        float lakeNoise = FractalNoise(
            new Vector2(worldX, worldY),
            biome.lakeScale,
            biome.lakeOctaves,
            biome.lakePersistence,
            biome.lakeLacunarity);
                  
        if (lakeNoise > biome.lakeThreshold)
            return biome.waterTile;

        int idx = Hash(worldX, worldY, seed) % biome.biomeTiles.Count;
        return biome.biomeTiles[idx];
    }


    TileBase GetTileAtPosition(Vector2Int position)
    {
        return groundLevel.GetTile(new Vector3Int(position.x, position.y, 0));
    }
    

    public void LoadChunk(Chunk chunk)
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
                    if (chunk.tiles[x, y].name == "WaterTileOcean" || chunk.tiles[x, y].name == "WaterTileSwamp")
                        waterLevel.SetTile(tilePosition, chunk.tiles[x, y]);
                    else
                        groundLevel.SetTile(tilePosition, chunk.tiles[x, y]);
                }
            }
            chunk.tilesSpawned = true;
        }
    }

    public void UnloadChunk(Chunk chunk)
    {
        chunk.chunkOBJ.SetActive(false);
        chunk.isLoaded = false;
        LoadedChunks.Remove(chunk);
    }

    public Vector2Int GetChunkPosition(Vector3 worldPosition)
    {
        // Convert world position to chunk position
        int chunkX = Mathf.FloorToInt(worldPosition.x / chunkSize);
        int chunkY = Mathf.FloorToInt(worldPosition.y / chunkSize);
        return new Vector2Int(chunkX, chunkY);
    }
    
    public Vector2Int GetChunkPosition(Vector2Int worldPosition)
    {
        // Convert world position to chunk position
        int chunkX = Mathf.FloorToInt((float)worldPosition.x / chunkSize);
        int chunkY = Mathf.FloorToInt((float)worldPosition.y / chunkSize);
        return new Vector2Int(chunkX, chunkY);
    }
    
    public void ResetWorld()
    {
        groundLevel.ClearAllTiles();
        waterLevel.ClearAllTiles();
        // Clear all loaded chunks
        foreach (var chunk in chunks.Values)
        {
            UnloadChunk(chunk);
            Destroy(chunk.chunkOBJ);
        }
        chunks.Clear();
        
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
    
    public void ApplyChangesToChunk(Chunk chunk)
    {
        if (chunk == null || chunk.changes == null || chunk.changes.Count == 0 || chunk.occupiedTiles == null)
            return;

        foreach (var change in chunk.changes)
        {
            switch (change.type)
            {
                case ChangeType.Removed:
                    RemoveResourceAt(change.tile);
                    break;
            }
        }
    }
    
    void RemoveResourceAt(Vector2Int worldTile)
    {
        GameObject resource = BuildingGrid.instance.GetObjectAtPosition(worldTile);

        if (resource == null) return;

        if (resource.TryGetComponent(out Building b) && b.gridPosition == worldTile)
        {
            BuildingGrid.instance.FreeArea(worldTile, b.size);
            Destroy(resource);
        }
    }

    public void GenerateTileToBiomeDictionary()
    {
        tileToBiome = new Dictionary<TileBase, BiomeData>();
        foreach (var biome in biomeList)
        {
            // dla każdego lądowego kafelka
            foreach (var land in biome.biomeTiles)
                tileToBiome[land] = biome;
            // oraz dla wodnego
            tileToBiome[biome.waterTile] = biome;
        }
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
