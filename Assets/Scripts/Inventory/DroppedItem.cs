using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
     public string itemName;
     public int amount;

     public Animator animator;
     
     public SpriteRenderer spriteRenderer;


     private void Start()
     {
          animator.SetTrigger("Jump");
     }

     public void SetItem(ItemDataID itemData)
     {
          itemName = itemData.name;
          amount = itemData.amount;
          spriteRenderer.sprite = ItemDatabaseInstance.Instance.GetItemByName(itemName).ItemImage;
     }
}
