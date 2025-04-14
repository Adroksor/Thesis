using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public string chestID;
    public List<InventoryUI> items = new();

    private void Awake()
    {
        if (string.IsNullOrEmpty(chestID))
        {
            chestID = System.Guid.NewGuid().ToString(); // Unique chest ID
        }
    }
}
