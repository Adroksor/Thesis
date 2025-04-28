using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "InventorySystem/ItemData/Pickaxe")]
public class PickaxeItem : EquipmentItem
{
    
    public override void Equip(ItemUser user)
    {
        
    }

    public override void Unequip(ItemUser user)
    {
        throw new System.NotImplementedException();
    }

    public override void Use(ItemUser user ,ItemStack stack)
    {
        throw new System.NotImplementedException();
    }
}
