using UnityEngine;

[CreateAssetMenu(menuName = "InventorySystem/ItemData/FoodItem")]
public class FoodItem : ItemData
{
    [Header("Food variables")]
    public int   hungerAmount = 5;

    public float eatCooldown  = 1.0f;

    [System.NonSerialized]
    private float _nextEatTime;

    public override bool Use(ItemUser user, ItemStack stack)
    {
        if (Time.time < _nextEatTime)
            return false;

        bool eaten = user.stats.RestoreHunger(hungerAmount);
        if (!eaten) 
            return false;

        _nextEatTime = Time.time + eatCooldown;
        return true;
    }
}