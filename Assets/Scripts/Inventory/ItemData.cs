using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "InventorySystem/ItemData")]
public abstract class ItemData : ScriptableObject
{
    public int ID => GetInstanceID();
    [field: SerializeField] 
    public int MaxStackSize { get; set; } = 20;
    [field: SerializeField]
    public string Name { get; set; }
    [field: SerializeField]
    public ItemCategory Itemtype { get; set; }
    [field: SerializeField]
    public Sprite ItemImage { get; set; }
    [field: SerializeField]
    [field: TextArea]
    public string Description { get; set; }

    public abstract bool Use(ItemUser user, ItemStack stack);
}

public enum ItemCategory
{
    None,
    Material,
    Weapon,
    Armor,
    Placeable,
    Consumable,
    Tool
}