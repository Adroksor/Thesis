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
        bool eaten = user.stats.RestoreHunger(hungerAmount);
        if (eaten)
        {
            InventoryData hotbar = InventoryManager.instance.playerInventory.hotbarData;
            int index = hotbar.GetIndexOfItem(this);
            
            hotbar.SubtrackItemFromStack(index, InventoryManager.instance.hotbarInventoryUI);
            
        }
    }
}
