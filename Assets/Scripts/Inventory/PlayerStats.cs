using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public float range;
    
    [Header("UI Elements")]
    public Image healthBar;
    public Image hungerBar;

    private void Start()
    {
        UpdateBars();
    }

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
        UpdateHungerBar();
        return true;
    }

    public void UpdateHungerBar()
    {
        hungerBar.fillAmount = (float)hunger / (float)maxHunger;
    }

    public void UpdateHealthBar()
    {
        healthBar.fillAmount = (float)health / (float)maxHealth;
    }

    public void UpdateBars()
    {
        UpdateHealthBar();
        UpdateHungerBar();
    }
}
