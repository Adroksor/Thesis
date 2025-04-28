using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "InventorySystem/ItemData/FoodItem")]
public class FoodItem : ItemData
{
    [Header("Food variables")]
    public int hungerAmount;
    public override void Use(ItemUser user, ItemStack stack)
    {
        user.stats.RestoreHunger(hungerAmount);
    }
}
