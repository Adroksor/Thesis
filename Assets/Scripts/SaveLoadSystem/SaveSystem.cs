using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR;

public class SaveSystem
{
    private static SaveData _saveData = new SaveData();
    
    
    [System.Serializable]
    public struct SaveData
    {
        public PlayerPositionData playerSaveData;
        public BuildingSaveData buildingSaveData;
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
        _saveData.buildingSaveData.buildingPositions = new List<BuildingPositionData>();

        foreach (var obj in GameManager.instance.objects)
        {
            if (obj.TryGetComponent(out Building building))
            {
                BuildingPositionData posData = new BuildingPositionData();
                building.Save(ref posData);                      // calls your method
                _saveData.buildingSaveData.buildingPositions.Add(posData);
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
        
        foreach (var posData in _saveData.buildingSaveData.buildingPositions)
        {
            // choose the prefab however you like (one prefab, or look up by type)
            GameObject prefab = GameManager.instance.GetObjectByName(posData.buildingName);
            GameObject obj = GameObject.Instantiate(prefab, posData.position, Quaternion.identity);

            if (obj.TryGetComponent(out Building building))
            {
                building.Load(posData);                 // sets the transform.position
                GameManager.instance.objects.Add(obj);  // track it again for next save
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
public struct BuildingPositionData
{
    public Vector2 position;
    public string buildingName;
}

[System.Serializable]
public struct BuildingSaveData
{
    public List<BuildingPositionData> buildingPositions;
}