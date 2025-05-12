using UnityEngine;

[CreateAssetMenu(menuName = "InventorySystem/ItemData/FoodItem")]
public class FoodItem : ItemData
{
    [Header("Food variables")]
    public int   hungerAmount = 5;

    public float eatCooldown  = 1.0f;

    [System.NonSerialized]
    private float _nextEatTime = -1f;

    public override bool Use(ItemUser user, ItemStack stack)
    {
        if (_nextEatTime < 0f)
        {
            _nextEatTime = Time.time + eatCooldown;
            return false;
        }
        if (Time.time < _nextEatTime)
            return false;

        if (!user.stats.RestoreHunger(hungerAmount))
            return false;

        _nextEatTime = Time.time + eatCooldown;
        return true;
    }
}