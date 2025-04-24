using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

public class SaveSystem
{
    private static SaveData _saveData = new SaveData();
    
    
    [System.Serializable]
    public struct SaveData
    {
        public PlayerPositionData playerSaveData;
        public List<StaticObjectData> buildings;
        public List<ChestData> chests;
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
        GameManager.instance.Save(ref _saveData.playerSaveData);
        
        
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
        GameManager.instance.Load(_saveData.playerSaveData);
        
        
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

        foreach (var chestData in _saveData.chests)
        {
            GameObject prefab = GameManager.instance.GetObjectByName(chestData.buildingName);
            GameObject obj = GameObject.Instantiate(prefab, chestData.position, Quaternion.identity);
            
            if (obj.TryGetComponent(out Chest building))
            {
                building.Load(chestData);
                GameManager.instance.chests.Add(obj);
            }
        }
    }
}



[System.Serializable]
public struct PlayerPositionData
{
    public Vector2 position;
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