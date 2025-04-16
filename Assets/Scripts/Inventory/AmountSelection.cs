using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmountSelection : MonoBehaviour
{
    public int minimum = 1;
    public int maximum = 100;

    public int amount;
    
    public Action<int> onAmountChanged;

    public void IncreaseAmount(int amount)
    {
        this.amount += amount;
        this.amount = Mathf.Clamp(this.amount, minimum, this.maximum);
        onAmountChanged?.Invoke(this.amount);
    }

    public void DecreaseAmount(int amount)
    {
        this.amount -= amount;
        this.amount = Mathf.Clamp(this.amount, minimum, this.maximum);
        onAmountChanged?.Invoke(this.amount);
    }

    public void SetAmount(int amount)
    {
        this.amount = amount;
        onAmountChanged?.Invoke(this.amount);
    }
}
