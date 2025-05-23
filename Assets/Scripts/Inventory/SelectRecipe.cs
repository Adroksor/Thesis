using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SelectRecipe : MonoBehaviour
{
    public FurnaceUI furnaceUI;
    public WorkbenchUI workbenchUI;
    public RecipeData selectedRecipe;
    public AmountSelection amountSelection;


    public void ButtonStart()
    {
        StartSmelting();
    }

    public void StartSmelting()
    {
        if (furnaceUI != null)
        {
            furnaceUI.furnace.ConsumeRecipeIngredients(selectedRecipe, amountSelection.amount);
            furnaceUI.furnace.StartSmelting(selectedRecipe, amountSelection.amount);
            amountSelection.SetAmount(1);
            InventoryManager.instance.FurnaceUI.SetActive(false);
            InventoryManager.instance.playerInventory.CloseInventory();
        }

        if (workbenchUI != null)
        {
            workbenchUI.workbench.ConsumeRecipeIngredients(selectedRecipe, amountSelection.amount);
            workbenchUI.workbench.StartCrafting(selectedRecipe, amountSelection.amount);
            amountSelection.SetAmount(1);
            InventoryManager.instance.WorkbenchUI.SetActive(false);
            InventoryManager.instance.playerInventory.CloseInventory();
        }

    }
}