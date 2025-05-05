using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "InventorySystem/ItemData/Pickaxe")]
public class PickaxeItem : EquipmentItem
{
    public float attackSpeed = 0.45f;    // seconds between swings

    private float _nextUseTime;          // item‑side timer

    public override bool Use(ItemUser user, ItemStack stack)
    {
        if (Time.time < _nextUseTime) return false;   // still cooling

        _nextUseTime = Time.time + attackSpeed;       // set next ready time
        SwingPickaxe(user);
        return true;
    }

    public void SwingPickaxe(ItemUser user)
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (new Vector2(mouseWorld.x, mouseWorld.y) - GameManager.instance.playerPosition);
        dir = dir.normalized;
        
        RaycastHit2D hit = Physics2D.Raycast(
            origin: GameManager.instance.playerPosition,
            direction: dir,
            distance: user.stats.range,
            layerMask: LayerMask.GetMask("Interactable"));

        if (hit.collider != null)
        {
            var building = hit.collider.GetComponentInParent<Building>();
            if (building != null && !building.isGhost)
            {
                building.stats.TakeDamage(damage);
            }
        }
    }
}
