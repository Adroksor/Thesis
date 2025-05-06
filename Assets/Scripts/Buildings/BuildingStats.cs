using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BuildingStats : MonoBehaviour
{
    public int health;
    public int maxHealth;

    public Action destroyMe;
    
    public void TakeDamage(int damage)
    {
        TweenHelper.PlayBounce(transform);
        health -= damage;
        if (health <= 0)
        {
            destroyMe?.Invoke();
        }
    }
}
