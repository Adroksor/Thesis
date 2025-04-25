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
        _saveData.worldSeed = GameManager.instance.seed;
        
        // Static Buildings
        _saveData.buildings = new List<StaticObjectData>();
        foreach (var obj in GameManager.instance.objects)
        {
            if (obj.TryGetComponent(out staticObject building))
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
        GameManager.instance.seed = _saveData.worldSeed;
        WorldGenerator.instance.ResetWorld();
        
        // Static Buildings
        foreach (var posData in _saveData.buildings)
        {
            GameObject prefab = GameManager.instance.GetObjectByName(posData.buildingName);
            GameObject obj = GameObject.Instantiate(prefab, posData.position, Quaternion.identity);

            if (obj.TryGetComponent(out staticObject building))
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
            
            if (obj.TryGetComponent(out Furnace building))
            {
                building.Load(furnaceData);
                GameManager.instance.furnaces.Add(obj);
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
public struct BuildingSaveData
{
    public List<StaticObjectData> buildingPositions;
}

[System.Serializable]
public struct ChestData             // chest = building + inventory
{
    public Vector2 position;
    public string buildingName;
    public List<SlotSaveData> inventory;
}

[System.Serializable]
public struct SlotSaveData          // <-- only used for persistence
{
    public int    slotIndex;        // key in the dictionary
    public string itemName;
    public int    amount;
}

[System.Serializable]
public struct FurnaceData             // chest = building + inventory
{
    public Vector2 position;
    public string buildingName;
}