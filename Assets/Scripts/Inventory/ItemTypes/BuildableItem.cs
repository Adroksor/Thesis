using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "InventorySystem/ItemData/BuildableItem")]
public class BuildableItem : ItemData
{
    public GameObject buildingPrefab;
    public override void Use(ItemUser user, ItemStack stack)
    {
        bool placed =  BuildingPlacer.instance.PlaceBuilding(buildingPrefab);
        if (placed)
        {
            Hotbar hotbar = InventoryManager.instance.playerInventory.hotbar;
            InventoryData hotbarData = InventoryManager.instance.playerInventory.hotbarData;
            
            int index = hotbar.itemIndex;
            hotbarData.SubtrackItemFromStack(index, InventoryManager.instance.hotbarInventoryUI);
        }
    }
}
