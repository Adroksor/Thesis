
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR // Important: Use UnityEditor only inside the editor
using UnityEditor;
using System.IO; // Needed for Path manipulation
#endif

[CreateAssetMenu(menuName = "InventorySystem/Database")]

public class ItemDatabase : ScriptableObject
{
    public List<ItemData> items;
    public ItemData missingItem;
    
    public ItemData GetItemByID(int id)
    {
        return items.FirstOrDefault(item => item.ID == id) ?? missingItem;
    }

    public ItemData GetItemByType(ItemType name)
    {
        return items.FirstOrDefault(item => item.name == name.ToString()) ?? missingItem;
    }
    
    public ItemData GetItemByname(string name)
    {
        return items.FirstOrDefault(item => item.name == name) ?? missingItem;
    }
    
    public void LoadItemsFromFolder(string folderPath = "")
    {
#if UNITY_EDITOR
        Debug.Log($"Attempting to load ItemData assets from: {folderPath}");

        items.Clear();

        string[] guids = AssetDatabase.FindAssets($"t:{nameof(ItemData)}", new[] { folderPath });

        if (guids.Length == 0)
        {
            Debug.LogWarning($"No ItemData assets found in folder: {folderPath}");
        }
        else
        {
            Debug.Log($"Found {guids.Length} ItemData assets.");
        }

        foreach (string guid in guids)
        {
            // Convert the GUID to an asset path
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            // Load the asset at the path
            ItemData item = AssetDatabase.LoadAssetAtPath<ItemData>(assetPath);

            if (item != null && item.name != "MissingItem")
            {
                items.Add(item);
            }
            else
            {
                Debug.LogWarning($"Failed to load asset at path: {assetPath}");
            }
        }

        EditorUtility.SetDirty(this);
        Debug.Log($"Finished loading. Total items in database: {items.Count}");
#else
        // This code will run in a build, where AssetDatabase is not available
        Debug.LogError("LoadItemsFromFolder can only be called from the Unity Editor.");
#endif
    }

    // Your [ContextMenu] function still works for the gear icon menu
    [ContextMenu("Test Context Menu")]
    private void MyButtonFunction()
    {
        Debug.Log($"Context menu button pressed! Item count: {items?.Count ?? 0}");
    }
}
