using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "InventorySystem/ItemData")]
public class ItemData : ScriptableObject
{
    public int ID => GetInstanceID();
    [field: SerializeField]
    public bool isStackable { get; set; }
    [field: SerializeField] 
    public int MaxStackSize { get; set; } = 20;
    [field: SerializeField]
    public string Name { get; set; }
    [field: SerializeField]
    public Sprite ItemImage { get; set; }
    [field: SerializeField]
    [field: TextArea]
    public string Description { get; set; }
    [field: SerializeField]
    public bool isPlacable { get; set; }
    

}