using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "InventorySystem/ItemData/FoodItem")]
public class FoodItem : ItemData
{
    [Header("Food variables")]
    public int hungerAmount;
    public override bool Use(ItemUser user, ItemStack stack)
    {
        bool eaten = user.stats.RestoreHunger(hungerAmount);
        return eaten;
    }
}
