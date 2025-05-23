using UnityEngine;

[CreateAssetMenu(menuName = "InventorySystem/ItemData")]
public abstract class ItemData : ScriptableObject
{
    [field: SerializeField] 
    public int MaxStackSize = 20;
    [field: SerializeField] 
    public string Name;
    [field: SerializeField] 
    public ItemCategory Itemtype;
    [field: SerializeField] 
    public Sprite ItemImage;
    [field: SerializeField] [field: TextArea]
    public string Description;

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