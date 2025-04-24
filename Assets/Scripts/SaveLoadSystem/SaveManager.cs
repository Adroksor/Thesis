
using System.Collections.Generic;
using UnityEngine;
public class SaveManager : MonoBehaviour
{
    public Dictionary<Vector2Int, Chunk> modifiedChunks = new Dictionary<Vector2Int, Chunk>();

    public static SaveManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        
    }

    public void SaveChunks()
    {
        WorldGenerator wg = WorldGenerator.instance;
        foreach (Transform child in wg.chunkList.transform)
        {
            if (child.childCount > 0)
            {
                Vector2Int chunkposition = new Vector2Int((int)child.transform.position.x, (int)child.transform.position.y);
                modifiedChunks.Add(chunkposition, wg.TryGetChunk(chunkposition));
            }
        }
        Debug.Log($"Saving {modifiedChunks.Count} chunks");
    }

    
}