using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine.UI;



public class Workbench : MonoBehaviour
{
    public CircularProgressBar progressBar;
    public Animator animator;

    public float craftTimePerItem = 5f;

    public BuildingInventory workbenchInventory;
    public Building building;
    
    private RecipeData currentRecipe;
    private int targetAmount;
    private int craftedAmount = 0;
    public bool isCrafting = false;
    private Coroutine craftingCoroutine;

    public void Start()
    {
        workbenchInventory = new BuildingInventory(5, 3);
        building.internalInventory = null;
        building.gettingRemoved += RemovingWorkbench;
        
        if (!building.isGhost)
        {
            GameManager.instance.workbenches.Add(gameObject);
        }
    }
    
    public void StartCrafting(RecipeData recipe, int amount)
    {
        if (isCrafting || recipe == null || amount <= 0)
        {
            Debug.LogWarning("Workbench: Cannot start crafting.");
            return;
        }
        progressBar.gameObject.SetActive(true);
        
        Image progressImage = progressBar.transform.Find("ItemIcon").GetComponent<Image>();
        if (progressImage != null)
        {
            progressImage.sprite = recipe.Output.item.ItemImage;
        }

        currentRecipe = recipe;
        targetAmount = amount;
        craftedAmount = 0;

        workbenchInventory.Clear();

        for (int i = 0; i < currentRecipe.Input.Count; i++)
        {
            var input = currentRecipe.Input[i];
            ItemStack stack = new ItemStack
            {
                item = input.item,
                amount = input.amount * amount
            };
            workbenchInventory.SetInput(i, stack);

        }
        craftingCoroutine = StartCoroutine(SmeltingRoutine());
    }

    private IEnumerator SmeltingRoutine()
    {
        while (craftedAmount < targetAmount)
        {
            bool currentItemFinished = false;

            progressBar.StartProgress(craftTimePerItem, () =>
            {
                currentItemFinished = true;
                TweenHelper.PlayBounce(transform);
                building.DropItem(new ItemStack{item = currentRecipe.Output.item, amount = currentRecipe.Output.amount});
            });
            yield return new WaitUntil(() => currentItemFinished);
            foreach (var input in currentRecipe.Input)
            {
                int index = workbenchInventory.inputSlots.FindIndex(s => s.item == input.item);
                if (!string.IsNullOrEmpty(input.item.name))
                {
                    workbenchInventory.inputSlots[index] = new ItemStack
                    {
                        item = input.item,
                        amount = workbenchInventory.inputSlots[index].amount - input.amount
                    };
                }
            }
            craftedAmount++;

        }

        isCrafting = false;
        craftingCoroutine = null;
        progressBar.gameObject.SetActive(false);
    }

    public void OpenWorkbench()
    {
        if (!isCrafting && !building.isGhost)
        {
            InventoryManager.instance.playerInventory.OpenInventory();
            InventoryManager.instance.WorkbenchUI.SetActive(true);
            InventoryManager.instance.currentlyInteractedObject = gameObject;

            WorkbenchUI workbenchUI = InventoryManager.instance.WorkbenchUI.GetComponent<WorkbenchUI>();
            workbenchUI.workbench = this;
            workbenchUI.InitializeRecipes();
        }
    }

    public void CloseWorkbench()
    {
        InventoryManager.instance.WorkbenchUI.SetActive(false);
        InventoryManager.instance.currentlyInteractedObject = null;
    }

    public void StopCrafting()
    {
        if (!isCrafting) return;

        if (craftingCoroutine != null)
        {
            StopCoroutine(craftingCoroutine);
            craftingCoroutine = null;
        }

        progressBar?.StopProgress();
        isCrafting = false;
        currentRecipe = null;
        targetAmount = 0;
    }


    public void RemovingWorkbench(Building building)
    {
        foreach (var input in workbenchInventory.inputSlots)
        {
            if (input.item && input.amount > 0)
            {
                building.DropItem(new ItemStack{item = input.item, amount = input.amount});
            }
        }
        workbenchInventory.Clear();
        GameManager.instance.workbenches.Remove(gameObject);
    }
    
    public void ConsumeRecipeIngredients(RecipeData recipe, int amount)
    {
        foreach (ItemStack input in recipe.Input)
        {
            InventoryManager.instance.TryRemoveItemsFromPlayerData(input.item, input.amount * amount);
        }
    }
    
    public void Save(ref WorkbenchData data)
    {
        data.position = transform.position;
        data.buildingName = transform.name;

    }

    public void Load(WorkbenchData data)
    {
        transform.position = data.position;
        building.isGhost = false;
    }

}