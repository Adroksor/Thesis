using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingStats : MonoBehaviour
{
    public int health;
    public int maxHealth;

    public Action destroyMe;
    
    public void TakeDamage(int damage)
    {
        health -= damage;
        
        if (health <= 0)
        {
            destroyMe?.Invoke();
        }
    }
}
