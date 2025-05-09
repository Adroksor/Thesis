using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem
{
    private static SaveData _saveData = new SaveData();
    
    [System.Serializable]
    public struct SaveData
    {
        public int worldSeed;
        public PlayerData playerSaveData;
        public List<StaticObjectData> buildings;
        public List<ChestData> chests;
        public List<FurnaceData> furnaces;
        public List<EntityData> entities;
        public List<ChunkSave> modifiedChunks;
    }
    
    public static string SaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/save" + ".save";
        return saveFile;
    }

    public static void Save()
    {
        Debug.Log("Save called");
        HandleSaveData();
        
        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(_saveData, true));
    }

    private static void HandleSaveData()
    {
        // Player
        PlayerData playerData = new PlayerData();
        GameManager.instance.player.TryGetComponent(out Player player);
        player.Save(ref playerData);
        _saveData.playerSaveData = playerData;
        
        // World
        _saveData.worldSeed = WorldGenerator.instance.seed;
        
        _saveData.modifiedChunks = new List<ChunkSave>();
        foreach (var cs in WorldGenerator.instance.chunks)
        {
            Chunk c = cs.Value;
            if (c.changes.Count == 0) continue;

            _saveData.modifiedChunks.Add(new ChunkSave {
                coord   = c.position,
                changes = new List<ResourceChange>(c.changes)
            });
        }

        
        // Static Buildings
        _saveData.buildings = new List<StaticObjectData>();
        foreach (var obj in GameManager.instance.objects)
        {
            if (obj.TryGetComponent(out StaticObject building))
            {
                StaticObjectData posData = new StaticObjectData();
                building.Save(ref posData);
                _saveData.buildings.Add(posData);
            }
        }
        
        // Chests
        _saveData.chests = new List<ChestData>();
        foreach (var obj in GameManager.instance.chests)
        {
            if (obj.TryGetComponent(out Chest building))
            {
                ChestData posData = new ChestData();
                building.Save(ref posData);
                _saveData.chests.Add(posData);
            }
        }
        // Furnaces
        _saveData.furnaces = new List<FurnaceData>();
        foreach (var obj in GameManager.instance.furnaces)
        {
            if (obj.TryGetComponent(out Furnace building))
            {
                FurnaceData posData = new FurnaceData();
                building.Save(ref posData);
                _saveData.furnaces.Add(posData);
            }
        }
        // Entities
        _saveData.entities = new List<EntityData>();
        foreach (var obj in GameManager.instance.entitiesPigs)
        {
            if (obj.TryGetComponent(out EntityStatus entity))
            {
                EntityData posData = new EntityData();
                entity.Save(ref posData);
                _saveData.entities.Add(posData);
            }
        }
    }

    public static void Load()
    {
        Debug.Log("Load called");
        
        string saveContent = File.ReadAllText(SaveFileName());
        _saveData = JsonUtility.FromJson<SaveData>(saveContent);
        
        HandleLoadData();
    }
    
    private static void HandleLoadData()
    {
        GameManager.instance.chests.Clear();
        GameManager.instance.furnaces.Clear();
        GameManager.instance.objects.Clear();

        
        
        // Player
        GameManager.instance.player.TryGetComponent(out Player player);
        player.Load(_saveData.playerSaveData);
        
        // World
        WorldGenerator wg = WorldGenerator.instance;
        wg.seed = _saveData.worldSeed;
        
        if (_saveData.modifiedChunks != null && _saveData.modifiedChunks.Count > 0)
        {
            foreach (ChunkSave cs in _saveData.modifiedChunks)
            {
                Chunk chunk = wg.TryGetChunk(cs.coord);
                if (chunk == null)
                {
                    chunk = wg.GenerateChunk(cs.coord);
                    wg.LoadChunk(chunk);
                    wg.SpawnResourcesForChunk(chunk);
                }
                chunk.changes.Clear();
                chunk.changes.AddRange(cs.changes);
                wg.ApplyChangesToChunk(chunk);
                
                wg.UnloadChunk(chunk);
            }
        }
        
        // Static Buildings
        foreach (var posData in _saveData.buildings)
        {
            GameObject prefab = GameManager.instance.GetObjectByName(posData.buildingName);
            GameObject obj = GameObject.Instantiate(prefab, posData.position, Quaternion.identity);
            obj.name = posData.buildingName;

            if (obj.TryGetComponent(out StaticObject building))
            {
                building.Load(posData);
                GameManager.instance.objects.Add(obj);
            }
        }

        // Chests
        foreach (var chestData in _saveData.chests)
        {
            GameObject prefab = GameManager.instance.GetObjectByName(chestData.buildingName);
            if(prefab == null)
                continue;
            GameObject obj = GameObject.Instantiate(prefab, chestData.position, Quaternion.identity);
            obj.name = chestData.buildingName;

            if (obj.TryGetComponent(out Chest building))
            {
                building.Load(chestData);
                GameManager.instance.chests.Add(obj);
            }
        }
        
        // Furnaces
        foreach (var furnaceData in _saveData.furnaces)
        {
            GameObject prefab = GameManager.instance.GetObjectByName(furnaceData.buildingName);
            if(prefab == null)
                continue;
            GameObject obj = GameObject.Instantiate(prefab, furnaceData.position, Quaternion.identity);
            obj.name = furnaceData.buildingName;

            if (obj.TryGetComponent(out Furnace building))
            {
                building.Load(furnaceData);
                GameManager.instance.furnaces.Add(obj);
            }
        }
        
        // Entities
        foreach (var entityData in _saveData.entities)
        {
            GameObject prefab = GameManager.instance.GetEntitytByName(entityData.entityName);
            if(prefab == null)
                continue;
            GameObject obj = GameObject.Instantiate(prefab, entityData.position, Quaternion.identity);
            obj.name = entityData.entityName;
            if (obj.TryGetComponent(out EntityStatus entity))
            {
                entity.Load(entityData);
                GameManager.instance.entitiesPigs.Add(obj);
            }
        }
    }
}



[System.Serializable]
public struct PlayerData
{
    public Vector2 position;
    public List<SlotSaveData> inventory;
    public List<SlotSaveData> hotbar;
}

[System.Serializable]
public struct StaticObjectData
{
    public Vector2 position;
    public string buildingName;
}

[System.Serializable]
public struct ChestData
{
    public Vector2 position;
    public string buildingName;
    public List<SlotSaveData> inventory;
}

[System.Serializable]
public struct SlotSaveData
{
    public int    slotIndex;
    public string itemName;
    public int    amount;
}

[System.Serializable]
public struct FurnaceData
{
    public Vector2 position;
    public string buildingName;
}

[System.Serializable]
public struct EntityData
{
    public Vector2 position;
    public string entityName;
}

[System.Serializable]
public struct FilledChunksData
{
    public Vector2Int position;
}

[System.Serializable]
public struct ResourceChange
{
    public Vector2Int tile;         // world tile coordinate
    public ChangeType type;         // Removed, HealthDelta, RegrowTimer …
    public float      value;        // hp left, time remaining, etc.
}

public enum ChangeType { Removed, HealthDelta, RegrowTimer }

[System.Serializable]
public struct ChunkSave
{
    public Vector2Int            coord;    // chunk grid coord
    public List<ResourceChange>  changes;  // per-tile deltas
}


