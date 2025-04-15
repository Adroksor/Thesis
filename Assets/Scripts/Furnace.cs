using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic; // Required for List

// Assuming RecipeData, InventoryUI, InventoryManager, ItemData exist elsewhere
// using YourNamespace; // If they are in a namespace

public class Furnace : MonoBehaviour
{
    public List<RecipeData> recipes; // Assign Recipes in Inspector
    public CircularProgressBar progressBar; // Assign CircularProgressBar in Inspector
    public PlayerInventory playerInventory; // This will be assigned from InventoryManager
    public Animator animator; // Assign Animator in Inspector

    public float smeltTimePerItem = 5f; // Time to smelt one unit of the output item

    private RecipeData currentRecipe;
    private int targetAmount; // How many items the player *requested* to smelt in this batch
    private int smeltedAmount = 0; // How many items have *actually* been smelted in the current batch
    private bool isSmelting = false;
    private Coroutine smeltingCoroutine;

    public void Start()
    {
        if (recipes != null && recipes.Count > 0 && recipes[0] != null)
        {
            StartSmelting(recipes[0], 10); // Example: Smelt 10 items of the first recipe
        }
        else
        {
             Debug.LogWarning("Furnace: No recipes assigned or the first recipe is invalid.");
        }
        
        playerInventory = InventoryManager.instance.playerInventoryScript;
    }
    public void StartSmelting(RecipeData recipe, int amount)
    {
        if (isSmelting)
        {
            Debug.LogWarning("Furnace is already smelting.");
            return;
        }
        if (recipe == null)
        {
             Debug.LogError("Furnace: Cannot start smelting with a null recipe.");
             return;
        }
         if (amount <= 0)
        {
             Debug.LogWarning("Furnace: Smelting amount must be positive.");
             return;
        }

        // --- TODO: Resource Check & Consumption ---
        // Before starting, check if the playerInventory has enough input materials
        // specified in recipe.Inputs.
        // If yes, consume them. If not, return and potentially notify the player.
        // Example structure:
        // if (!InventoryManager.instance.CheckAndConsumeInputs(recipe.Inputs, playerInventory))
        // {
        //    Debug.Log("Not enough input materials to start smelting!");
        //    return;
        // }
        // -----------------------------------------

        currentRecipe = recipe;
        targetAmount = amount;
        smeltedAmount = 0; // Reset count for the new batch

        Debug.Log($"Starting to smelt {targetAmount} of {currentRecipe.Output.Item.name}");
        smeltingCoroutine = StartCoroutine(SmeltingRoutine());
    }

    private IEnumerator SmeltingRoutine()
    {
        isSmelting = true;

        while (smeltedAmount < targetAmount)
        {
            bool currentItemFinished = false;

            progressBar.StartProgress(smeltTimePerItem, () => {
                currentItemFinished = true;
                Bounce();                   
            });

            yield return new WaitUntil(() => currentItemFinished);

            Debug.Log($"Finished smelting one item ({smeltedAmount + 1}/{targetAmount}).");

            smeltedAmount++;
        }

        Debug.Log($"Smelting batch finished. Total smelted: {smeltedAmount}");
        isSmelting = false;
        smeltingCoroutine = null;
    }

    public void GivePlayerSmeltedItems()
    {
        if (currentRecipe == null || currentRecipe.Output == null || currentRecipe.Output.Item == null)
        {
            Debug.LogWarning("Cannot give items: No valid recipe/output defined.");
            smeltedAmount = 0;
            return;
        }

        if (smeltedAmount > 0)
        {
            int totalToGive = smeltedAmount * currentRecipe.Output.Amount;
            Debug.Log($"Attempting to give {totalToGive} of {currentRecipe.Output.Item.name} (Smelted: {smeltedAmount}, Recipe Output: {currentRecipe.Output.Amount})");
            
            InventoryManager.instance.TryAddItemToInventoryData(currentRecipe.Output.Item, totalToGive, playerInventory.inventoryData);
            targetAmount -= smeltedAmount;
            smeltedAmount = 0;

        }
        else
        {
            Debug.Log("No items currently smelted to give.");
        }
    }
    public void StopSmelting()
    {
        if (!isSmelting) return;

        Debug.Log("Stopping smelting process...");

        if (smeltingCoroutine != null)
        {
            StopCoroutine(smeltingCoroutine);
            smeltingCoroutine = null;
        }
         if (progressBar != null)
        {
             progressBar.StopProgress();
        }

        isSmelting = false;

        GivePlayerSmeltedItems();

        // --- TODO: Refund Logic ---
        // Calculate remaining items that were *requested* but not *started* or *finished*.
        // Refund the corresponding input materials based on currentRecipe.Inputs.
        // int remainingToSmelt = targetAmount - smeltedAmount; // Note: smeltedAmount is now 0 if GivePlayerSmeltedItems succeeded
        // Need to potentially track consumed resources separately if refunding is complex.
        // For simplicity now, we just stop and give what was finished.
        // --------------------------

        currentRecipe = null; // Clear the recipe
        targetAmount = 0;
    }

    public void OnPlayerInteract()
    {
        GivePlayerSmeltedItems();

        // Optional: If not smelting, maybe open a UI to select a recipe?
        // if (!isSmelting) {
        //     OpenFurnaceUI();
        // }
    }

    public void Bounce()
    {
        if (animator != null)
        {
             animator.SetTrigger("Bounce");
        }
    }
}