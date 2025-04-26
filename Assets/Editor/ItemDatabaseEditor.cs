// ItemDatabaseEditor.cs (Place this inside the Assets/Editor folder)
using UnityEngine;
using UnityEditor; // Required for editor scripting

// Target the ItemDatabase ScriptableObject
[CustomEditor(typeof(ItemDatabase))]
public class ItemDatabaseEditor : Editor
{
    // Folder path variable - can be adjusted in the editor script itself
    private string _itemFolderPath = "Assets/Scripts/Inventory/Items"; // Default folder

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        ItemDatabase database = (ItemDatabase)target;
        
        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("Database Actions", EditorStyles.boldLabel);

        _itemFolderPath = EditorGUILayout.TextField("Items Folder Path", _itemFolderPath);

        if (GUILayout.Button($"Load Items"))
        {
            database.LoadItemsFromFolder(_itemFolderPath);
            ItemEnumGenerator.WriteEnum(database.items);
        }

        EditorGUILayout.Space(10);
        if (GUILayout.Button("Clear Item List"))
        {
             if (EditorUtility.DisplayDialog("Confirm Clear",
                 "Are you sure you want to remove all items from the list?", "Yes, Clear", "Cancel"))
             {
                 database.items.Clear();
                 EditorUtility.SetDirty(database); // Save the change
                 Debug.Log("Item list cleared via Custom Editor.");
             }
        }
    }
}