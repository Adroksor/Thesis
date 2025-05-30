using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class DroppedItem : MonoBehaviour
{
     public string itemName;
     public int amount;

     public bool canPickup = true;

     public Animator animator;
     
     public SpriteRenderer spriteRenderer;
     
     public bool isBeingCollected = false;
     private float followDuration = 1.5f;
     private float followTimer = 0f;

     private float despawnTimer = 300;
     private Transform target;

     public GameObject floatingTextPrefab;


     private void Start()
     {
          animator.SetTrigger("Jump");
     }
     
     private void Update()
     {
          if (isBeingCollected)
          {
               followTimer += Time.deltaTime;

               if (target != null)
               {
                    // Move toward player
                    transform.position = Vector3.MoveTowards(
                         transform.position,
                         target.position,
                         10f * Time.deltaTime // speed
                    );

                    // Close enough to collect?
                    if (Vector2.Distance(transform.position, target.position) < 0.3f)
                    {
                         TryPickup();
                    }
               }
               if (followTimer >= followDuration)
               {
                    isBeingCollected = false;
                    target = null;
               }
          }
          despawnTimer -= Time.deltaTime;
          if (despawnTimer <= 0)
          {
               GameManager.instance.droppedItems.Remove(gameObject);
               Destroy(gameObject);
          }
     }
     
     public void StartCollecting(Transform player)
     {
          isBeingCollected = true;
          followTimer = 0f;
          target = player;
     }

     private void TryPickup()
     {
          ItemData item = ItemDatabaseInstance.instance.GetItemByname(itemName);
          int remaining = InventoryManager.instance.TryAddItemsToPlayerData(
               item,
               amount
          );
          if (remaining <= 0)
          {
               SpawnPickupText(item.Name, amount);
               GameManager.instance.droppedItems.Remove(gameObject);
               Destroy(gameObject);
          }
          else
          {
               amount = remaining;
          }
     }
     
     void SpawnPickupText(string itemName, int amount)
     {
          Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

          GameObject go = Instantiate(floatingTextPrefab, canvas.transform);
          RectTransform rt = (RectTransform)go.transform;
          Vector2 spawn = Random.insideUnitCircle * 40;
          rt.localPosition    = spawn;       // screen‑space position
          rt.localScale  = Vector3.one;

          var fx = go.GetComponent<PickupFloatingText>();
          fx.Init($"+{amount} {itemName}");
     }


     public void SetItem(ItemStack itemData)
     {
          itemName = itemData.item.name;
          gameObject.name = itemName;
          amount = itemData.amount;
          spriteRenderer.sprite = itemData.item.ItemImage;
          
          GameManager.instance.droppedItems.Add(gameObject);
     }
    
     public void Save(ref DroppedItemData data)
     {
          data.position = transform.position;
          data.itemName = transform.name;
          data.amount = amount;
     }

     public void Load(DroppedItemData data)
     {
          ItemStack stack = new ItemStack
          {
               item = ItemDatabaseInstance.instance.GetItemByname(data.itemName),
               amount = data.amount
          };
          SetItem(stack);
     }
}
