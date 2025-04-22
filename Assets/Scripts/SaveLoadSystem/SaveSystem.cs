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
    }
}



[System.Serializable]
public struct PlayerPositionData
{
    public Vector2 position;
}