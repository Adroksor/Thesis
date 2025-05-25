using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "InventorySystem/ItemData/BuildableItem")]
public class BuildableItem : ItemData
{
    public GameObject buildingPrefab;
    public bool occupyArea = true;
    public override bool Use(ItemUser user, ItemStack stack)
    {
        bool placed = false;
        if (!occupyArea)
        {
            placed = BuildingPlacer.instance.PlaceTile(buildingPrefab);
            return placed;
        }
        placed =  BuildingPlacer.instance.PlaceBuilding(buildingPrefab);
        return placed;
    }
}
