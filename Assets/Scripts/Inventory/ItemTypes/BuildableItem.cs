using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "InventorySystem/ItemData/BuildableItem")]
public class BuildableItem : ItemData
{
    public GameObject buildingPrefab;
    public override void Use(ItemUser user, ItemStack stack)
    {

        BuildingPlacer.instance.PlaceBuilding(buildingPrefab);
    }
}
