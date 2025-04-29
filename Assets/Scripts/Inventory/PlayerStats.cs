using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int health;
    public int maxHealth;
    public int stamina;
    public int maxStamina;
    public int speed;
    public int sprintSpeed;
    public int hunger;
    public int maxHunger;

    public void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth) health = maxHealth;
    }

    public bool RestoreHunger(int amount)
    {
        if (hunger == maxHunger)
            return false;
        
        hunger += amount;
        if (hunger > maxHunger) hunger = maxHunger;
        return true;
    }
}
