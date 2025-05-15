
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
    public List<RecipeData> furnaceRecipes;
    public List<RecipeData> workbenchRecipes;
    public ItemData missingItem;
    
    
    public ItemData GetItemByname(string name)
    {
        return items.FirstOrDefault(item => item.name == name) ?? missingItem;
    }
    
    public RecipeData GetFurnaceRecipeByname(string name)
    {
        return furnaceRecipes.FirstOrDefault(recipe => recipe.name == name);
    }

    public RecipeData GetWorkbenchByname(string name)
    {
        return workbenchRecipes.FirstOrDefault(recipe => recipe.name == name);
    }

#if UNITY_EDITOR
    public void LoadItemsFromFolder(string folderPath = "Assets/Items")
    {
        // pomijamy placeholder „MissingItem”
        LoadAssetsFromFolder<ItemData>(
            folderPath,
            items,
            new HashSet<string>{ "MissingItem" });

        EditorUtility.SetDirty(this);
    }
#endif
    
#if UNITY_EDITOR
    public void LoadRecipesFromFolder(string folderPath = "Assets/Scripts/Inventory/Recipes")
    {
        LoadAssetsFromFolder<RecipeData>($"{folderPath}/Furnace", furnaceRecipes);
        LoadAssetsFromFolder<RecipeData>($"{folderPath}/Workbench", workbenchRecipes);

        EditorUtility.SetDirty(this);
    }
#endif
    
    // Your [ContextMenu] function still works for the gear icon menu
    [ContextMenu("Test Context Menu")]
    private void MyButtonFunction()
    {
        Debug.Log($"Context menu button pressed! Item count: {items?.Count ?? 0}");
    }
    
#if UNITY_EDITOR

    /// <summary>Uniwersalny loader ScriptableObjectów z podanego folderu.</summary>
    /// <typeparam name="T">Klasa zasobu (musi dziedziczyć po ScriptableObject).</typeparam>
    /// <param name="folderPath">Ścieżka do folderu (Assets/…)</param>
    /// <param name="targetList">Lista, do której dodajemy wczytane assety.</param>
    /// <param name="skipNames">Opcjonalna lista nazw, które chcemy pominąć.</param>
    public static void LoadAssetsFromFolder<T>(
        string   folderPath,
        List<T>  targetList,
        HashSet<string> skipNames = null)
        where T : ScriptableObject
    {
        targetList.Clear();

        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { folderPath });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var asset   = AssetDatabase.LoadAssetAtPath<T>(path);

            if (asset == null) continue;
            if (skipNames != null && skipNames.Contains(asset.name)) continue;

            targetList.Add(asset);
        }

        Debug.Log($"[Loader] Załadowano {targetList.Count} obiektów typu {typeof(T).Name} z „{folderPath}”.");
    }
#endif

}