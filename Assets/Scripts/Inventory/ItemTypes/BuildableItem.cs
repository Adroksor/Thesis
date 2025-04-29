using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "InventorySystem/ItemData/BuildableItem")]
public class BuildableItem : ItemData
{
    public GameObject buildingPrefab;
    public override bool Use(ItemUser user, ItemStack stack)
    {
        bool placed =  BuildingPlacer.instance.PlaceBuilding(buildingPrefab);
        return placed;
    }
}
