using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "InventorySystem/Database")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemData> items;
    public ItemData missingItem;
    
    
    public ItemData GetItemByID(int id)
    {
        return items.FirstOrDefault(item => item.ID == id);
    }

    public ItemData GetItemByName(string name)
    {
        return items.FirstOrDefault(item => item.Name == name);
    }
    
    public bool TryGetItemByID(int id, out ItemData result)
    {
        result = items.FirstOrDefault(item => item.ID == id);
        return result != null;
    }

    public bool TryGetItemByName(string name, out ItemData result)
    {
        result = items.FirstOrDefault(item => item.Name == name);
        return result != null;
    }
}