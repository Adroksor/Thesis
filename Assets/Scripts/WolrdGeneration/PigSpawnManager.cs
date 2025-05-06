using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PigSpawnManager : MonoBehaviour
{
    [Header("Setup")]
    public GameObject pigPrefab;
    public WorldGenerator worldGen;          // ref to your chunk system
    public float spawnInterval = 30f;        // seconds between spawns
    public int   maxPigs       = 50;         // cap to avoid runaway

    readonly List<GameObject> livePigs = new();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(this);
    }
    public static PigSpawnManager Instance { get; private set; }

    void OnEnable()  => StartCoroutine(SpawnLoop());
    void OnDisable() => StopAllCoroutines();

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // skip if population full
            livePigs.RemoveAll(p => p == null);
            if (livePigs.Count >= maxPigs) continue;

            TrySpawnPig();
        }
    }

    void TrySpawnPig()
    {
        var loaded = worldGen.LoadedChunks;
        if (loaded.Count == 0) return;
        var chunk  = loaded[Random.Range(0, loaded.Count)];

        for (int attempt = 0; attempt < 10; attempt++)
        {
            
            TileBase tile = chunk.GetRandomTile(out int x, out int y);
            if(tile == null) continue;
            if(tile.name == "WaterTile") continue;
            Vector3 world  = new Vector3(x + 0.5f, y + 0.5f, 0);

            GameObject pig = Instantiate(pigPrefab, world, Quaternion.identity);
            livePigs.Add(pig);
            return;
        }
    }
}
