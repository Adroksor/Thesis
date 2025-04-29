using UnityEngine;


[CreateAssetMenu(menuName = "InventorySystem/ItemData/MaterialItem")]
public class MaterialItem : ItemData
{
    public override bool Use(ItemUser user, ItemStack stack)
    {
        return true;
    }
}
