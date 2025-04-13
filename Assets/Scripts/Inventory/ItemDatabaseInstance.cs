using UnityEngine;

public static class ItemDatabaseInstance
{
    private static ItemDatabase _instance;

    public static ItemDatabase Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<ItemDatabase>("ItemDatabase");
                if (_instance == null)
                {
                    Debug.LogError("ItemDatabase asset not found in Resources folder!");
                }
            }
            return _instance;
        }
    }
}