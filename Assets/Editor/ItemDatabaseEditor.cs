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
        // Draw the default inspector fields (items list, missingItem)
        DrawDefaultInspector();

        // Get a reference to the ItemDatabase instance being inspected
        ItemDatabase database = (ItemDatabase)target;

        // Add some visual space
        EditorGUILayout.Space(20);

        // Display a label for clarity
        EditorGUILayout.LabelField("Database Actions", EditorStyles.boldLabel);

        // Optionally, allow editing the target folder path in the inspector
        _itemFolderPath = EditorGUILayout.TextField("Items Folder Path", _itemFolderPath);

        // Create the button
        if (GUILayout.Button($"Load Items"))
        {
            // Call the public loading function on the database instance
            // Pass the path from the text field
            database.LoadItemsFromFolder(_itemFolderPath);

            // Optional: Log confirmation in the console
            Debug.Log("Load Items button clicked via Custom Editor.");
        }

        // Add more buttons or custom GUI elements here if needed
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