using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SelectRecipe : MonoBehaviour
{
    public Furnace furnace;

    public RecipeData selectedRecipe;
    public AmountSelection amountSelection;


    public void ButtonStart()
    {
        StartSmelting();
    }

    public void StartSmelting()
    {
        furnace.StartSmelting(selectedRecipe, amountSelection.amount);
        InventoryManager.instance.FurnaceUI.SetActive(false);

    }
    
}
