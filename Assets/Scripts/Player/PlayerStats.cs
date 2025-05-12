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

    
    public float tickInterval  = 2f;   // seconds between hunger ticks
    public int  hungerPerTick  = 1;    // how much to lose each tick
    public int  starvationDps  = 5;    // HP lost per second when hunger = 0

    float _nextTickTime;
    private void Start()
    {
        UpdateBars();
    }
    
    void Update()
    {
        if (Time.time >= _nextTickTime)
        {
            _nextTickTime = Time.time + tickInterval;
            DoTick();
        }
    }

    /* ───────────────────────────────────────────────────────────── */

    void DoTick()
    {
        if (hunger > 0)
        {
            RestoreHunger(-hungerPerTick);
        }
        else
        {
            TakeDamage(starvationDps);
            if (health == 0) Debug.Log("Dead");
        }
    }
    

    public void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth) health = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0) health = 0;
        UpdateHealthBar();
    }

    public bool RestoreHunger(int amount)
    {
        if (hunger == maxHunger && amount > 0)
            return false;
        
        hunger += amount;
        if (hunger >= maxHunger) hunger = maxHunger;
        if (hunger < 0) hunger = 0;
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
