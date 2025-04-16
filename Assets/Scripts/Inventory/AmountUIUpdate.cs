using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class AmountUIUpdate : MonoBehaviour
{
    public AmountSelection amountselection;

    public TextMeshProUGUI amountText;

    
    public void UpdateText(int amount)
    {
        amountText.text = amount.ToString();
    }

    private void OnEnable()
    {
        amountselection.onAmountChanged += UpdateText;
    }

    private void OnDisable()
    {
        amountselection.onAmountChanged -= UpdateText;
    }
}
