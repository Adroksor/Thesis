using UnityEngine;

[CreateAssetMenu(menuName = "InventorySystem/ItemData/FoodItem")]
public class FoodItem : ItemData
{
    [Header("Food variables")]
    public int hungerAmount = 0;

    public int healthAmount = 0;
    
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

        bool changed = false;

        if (hungerAmount > 0)
            changed |= user.stats.RestoreHunger(hungerAmount);

        if (healthAmount > 0)
            changed |= user.stats.Heal(healthAmount); 

        if (!changed)
            return false;

        _nextEatTime = Time.time + eatCooldown;
        return true;
    }
}