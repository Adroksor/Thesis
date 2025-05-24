using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class EntityStatus : MonoBehaviour
{
    
    public int health;
    public int maxHealth;
    public SpriteRenderer sr;
    public float flashDuration = 0.12f;
    
    public List<ItemStack> drops;
    
    public GameObject itemPrefab;

    public Action onDeath;
    public Action onDamage;

    private void Start()
    {
        onDeath += Die;
    }

    public void TakeDamage(int damage)
    {
        onDamage?.Invoke();
        TweenHelper.PlayBounce(transform);
        
        sr.DOKill();
        sr.DOColor(Color.red, flashDuration / 2f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
                sr.DOColor(Color.white, flashDuration / 2f)
                    .SetEase(Ease.InQuad));
        
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
    
    public void Save(ref EntityData data)
    {
        data.position = transform.position;
        data.entityName = transform.name;

    }

    public void Load(EntityData data)
    {
        transform.position = data.position;
    }
}
