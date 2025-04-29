using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "InventorySystem/ItemData/Pickaxe")]
public class PickaxeItem : EquipmentItem
{
    public override void Use(ItemUser user ,ItemStack stack)
    {
        SwingPickaxe();
    }

    public void SwingPickaxe()
    {
        Debug.Log("Swing Pickaxe");
    }
}
