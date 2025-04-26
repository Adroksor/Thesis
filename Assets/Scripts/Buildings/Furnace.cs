using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;



public class Furnace : MonoBehaviour
{
    public CircularProgressBar progressBar;
    public Animator animator;

    public float smeltTimePerItem = 5f;

    public FurnaceInventory furnaceInventory;
    public Building building;
    
    
    private RecipeData currentRecipe;
    private int targetAmount;
    private int smeltedAmount = 0;
    public bool isSmelting = false;
    private Coroutine smeltingCoroutine;

    public void Start()
    {
        furnaceInventory = new FurnaceInventory(5, 3);
        building.internalInventory = null;
        building.gettingRemoved += RemovingFurnace;

        if (!building.isGhost)
        {
            GameManager.instance.furnaces.Add(gameObject);
        }
    }
    
    public void StartSmelting(RecipeData recipe, int amount)
    {
        if (isSmelting || recipe == null || amount <= 0)
        {
            Debug.LogWarning("Furnace: Cannot start smelting.");
            return;
        }
        progressBar.gameObject.SetActive(true);
        
        Image progressImage = progressBar.transform.Find("ItemIcon").GetComponent<Image>();
        if (progressImage != null)
        {
            progressImage.sprite = recipe.Output.Item.ItemImage;
        }

        currentRecipe = recipe;
        targetAmount = amount;
        smeltedAmount = 0;

        furnaceInventory.Clear();

        for (int i = 0; i < currentRecipe.Input.Count; i++)
        {
            var input = currentRecipe.Input[i];
            furnaceInventory.SetInput(i, Enum.Parse<ItemType>(input.Item.Name), input.Amount * targetAmount);
        }

        smeltingCoroutine = StartCoroutine(SmeltingRoutine());
    }

    private IEnumerator SmeltingRoutine()
    {
        isSmelting = true;

        while (smeltedAmount < targetAmount)
        {
            bool currentItemFinished = false;

            progressBar.StartProgress(smeltTimePerItem, () =>
            {
                currentItemFinished = true;
                Bounce();
                building.DropItem(new ItemDataID{name = Enum.Parse<ItemType>(currentRecipe.Output.Item.Name), amount = currentRecipe.Output.Amount});
            });
            
            foreach (var input in currentRecipe.Input)
            {
                furnaceInventory.SubtractFromInput(input.Item.Name, input.Amount);
            }

            yield return new WaitUntil(() => currentItemFinished);
            smeltedAmount++;

        }

        isSmelting = false;
        smeltingCoroutine = null;
        progressBar.gameObject.SetActive(false);
    }

    public void OpenFurnace()
    {
        if (!isSmelting && !building.isGhost)
        {
            InventoryManager.instance.playerInventory.OpenInventory();
            InventoryManager.instance.FurnaceUI.SetActive(true);
            InventoryManager.instance.currentlyInteractedObject = gameObject;
            
            FurnaceUI furnaceUI = InventoryManager.instance.FurnaceUI.GetComponent<FurnaceUI>();
            furnaceUI.furnace = this;
            furnaceUI.InitializeRecipes();
        }
    }

    public void CloseFurnace()
    {
        InventoryManager.instance.FurnaceUI.SetActive(false);
        InventoryManager.instance.currentlyInteractedObject = null;
    }

    public void StopSmelting()
    {
        if (!isSmelting) return;

        if (smeltingCoroutine != null)
        {
            StopCoroutine(smeltingCoroutine);
            smeltingCoroutine = null;
        }

        progressBar?.StopProgress();
        isSmelting = false;
        currentRecipe = null;
        targetAmount = 0;
    }

    public void Bounce()
    {
        if (animator != null)
        {
             animator.SetTrigger("Bounce");
        }
    }

    public void RemovingFurnace(Building building)
    {
        // Return all inputs and outputs to internal inventory
        foreach (var input in furnaceInventory.inputSlots)
        {
            if (input.name != ItemType.None && input.amount > 0)
            {
                building.DropItem(new ItemDataID{name = input.name, amount = input.amount});
            }
        }
        furnaceInventory.Clear();
        GameManager.instance.furnaces.Remove(gameObject);
    }
    
    public void ConsumeRecipeIngredients(RecipeData recipe, int amount)
    {
        foreach (RecipeSlotData input in recipe.Input)
        {
            InventoryManager.instance.TryRemoveItemsFromInventoryData(input.Item, input.Amount * amount, InventoryManager.instance.playerInventory.inventoryData );
        }
    }
    
    public void Save(ref FurnaceData data)
    {
        data.position = transform.position;
        data.buildingName = Enum.Parse<ItemType>(transform.name);

    }

    public void Load(FurnaceData data)
    {
        transform.position = data.position;
        building.isGhost = false;
    }

}