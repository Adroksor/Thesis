using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
     private Transform target;


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
                         5f * Time.deltaTime // speed
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
     }
     
     public void StartCollecting(Transform player)
     {
          isBeingCollected = true;
          followTimer = 0f;
          target = player;
     }

     private void TryPickup()
     {
          int remaining = InventoryManager.instance.TryAddItemToInventoryData(
               ItemDatabaseInstance.instance.GetItemByName(itemName),
               amount,
               InventoryManager.instance.playerInventoryScript.inventoryData
          );

          if (remaining <= 0)
          {
               Destroy(gameObject);
          }
          else
          {
               amount = remaining;
          }
     }


     public void SetItem(ItemDataID itemData)
     {
          itemName = itemData.name;
          amount = itemData.amount;
          spriteRenderer.sprite = ItemDatabaseInstance.instance.GetItemByName(itemName).ItemImage;
     }
}
