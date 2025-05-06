using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EntityStatus : MonoBehaviour
{
    
    public int health;
    public int maxHealth;
    
    public List<ItemStack> drops;
    
    public GameObject itemPrefab;

    public Action onDeath;

    private void Start()
    {
        onDeath += Die;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if(health <= 0)
            onDeath?.Invoke();
    }

    public void Die()
    {
        DropItems(drops);
        Destroy(gameObject);
    }

    public void DropItems(List<ItemStack> itemsToDrop)
    {
        foreach (var item in itemsToDrop)
        {
            for (int i = 0; i < item.amount; i++)
            {
                if (!string.IsNullOrEmpty(item.item.name) || item.amount != 0)
                {
                    Vector3 positionOffset = Random.insideUnitCircle / 2;

                    positionOffset.z = 0;
                    positionOffset.y -= 0.15f;
                    GameObject dropped = Instantiate(itemPrefab, transform.position + positionOffset,
                        Quaternion.identity);

                    DroppedItem droppedScript = dropped.GetComponent<DroppedItem>();
                    if (droppedScript != null)
                    {
                        ItemStack toSet = new ItemStack
                        {
                            item = item.item,
                            amount = 1
                        };
                        droppedScript.SetItem(toSet);
                    }
                }
            }
        }
    }
}
