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
            Hotbar hotbar = InventoryManager.instance.playerInventory.hotbar;
            InventoryData hotbarData = InventoryManager.instance.playerInventory.hotbarData;
            int index = hotbar.itemIndex;
            
            hotbarData.SubtrackItemFromStack(index, InventoryManager.instance.hotbarInventoryUI);
            
        }
    }
}
