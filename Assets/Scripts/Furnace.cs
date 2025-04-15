using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnace : MonoBehaviour
{
    public List<RecipeData> recipes;
    public CircularProgressBar progressBar;
    public InventoryUI playerInventory;

    private RecipeData currentRecipe;
    private int targetAmount;
    private int smeltedAmount = 0;
    private bool isSmelting = false;
    private float smeltTimePerItem = 5;
    private Coroutine smeltingCoroutine;
    public Animator animator;
    

    public void Start()
    {
        playerInventory = InventoryManager.instance.playerInventory;
        progressBar.onTimeout += Bounce;

    }

    public void StartSmelting(RecipeData recipe, int amount)
    {
        if (isSmelting) return;

        currentRecipe = recipe;
        targetAmount = amount;
        smeltedAmount = 0;

        smeltingCoroutine = StartCoroutine(SmeltingRoutine());
    }

    private IEnumerator SmeltingRoutine()
    {
        isSmelting = true;

        while (smeltedAmount < targetAmount)
        {
            bool finished = false;
            progressBar.StartProgress(smeltTimePerItem, () => finished = true);

            // Wait until this one item is smelted
            yield return new WaitUntil(() => finished);

            smeltedAmount++;
        }

        GivePlayerSmeltedItems();
        isSmelting = false;
        currentRecipe = null;
    }

    public void GivePlayerSmeltedItems()
    {
        if (smeltedAmount > 0)
        {
            InventoryManager.instance.TryAddItemToInventory(currentRecipe.Output.Item, smeltedAmount * currentRecipe.Output.Amount, playerInventory);
            smeltedAmount = 0;
        }
    }

    public void StopSmelting()
    {
        if (!isSmelting) return;

        if (smeltingCoroutine != null)
            StopCoroutine(smeltingCoroutine);

        isSmelting = false;

        // Give smelted items so far
        GivePlayerSmeltedItems();

        // Refund remaining input materials (example — adapt to your actual resource system)
        int remaining = targetAmount - smeltedAmount;
        if (remaining > 0)
        {
            InventoryManager.instance.TryAddItemToInventory(currentRecipe.Output.Item, smeltedAmount * currentRecipe.Output.Amount, playerInventory);
        }

        currentRecipe = null;
    }

    public void OnPlayerInteract()
    {
        if (isSmelting)
        {
            // Give progress so far but keep going
            GivePlayerSmeltedItems();
        }
    }

    public void Bounce()
    {
        Debug.Log("called");
        animator.SetTrigger("Bounce");
    }
}
