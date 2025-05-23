using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "InventorySystem/ItemData/Pickaxe")]
public class PickaxeItem : EquipmentItem
{
    public float attackSpeed = 0.45f;

    [System.NonSerialized]
    private float _nextUseTime;

    public override bool Use(ItemUser user, ItemStack stack)
    {
        if (Time.time < _nextUseTime)
        {
            return false;
        }
        _nextUseTime = Time.time + attackSpeed;
        SwingPickaxe(user);
        return true;        
    }

    public void SwingPickaxe(ItemUser user)
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (new Vector2(mouseWorld.x, mouseWorld.y) - GameManager.instance.playerPosition);
        dir = dir.normalized;
        
        RaycastHit2D hit = Physics2D.Raycast( GameManager.instance.playerPosition,dir, 
                            user.stats.range, LayerMask.GetMask("Interactable", "Entity"));

        if (hit.collider != null)
        {
            var building = hit.collider.GetComponentInParent<Building>();
            if (building != null && !building.isGhost)
            {
                building.stats.TakeDamage(damage);
            }
            var entity = hit.collider.GetComponent<EntityStatus>();
            if (entity != null)
            {
                entity.TakeDamage(damage);
            }
        }
    }
}
