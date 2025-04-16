using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.UIElements; // Required for List


public class Furnace : MonoBehaviour
{
    public List<RecipeData> recipes;
    public CircularProgressBar progressBar;
    public PlayerInventory playerInventory;
    public Animator animator;

    public float smeltTimePerItem = 5f;

    public FurnaceInventory furnaceInventory;
    public Building building;
    
    public GameObject recipeUI;
    public GameObject item;
    public Button button;
    public SelectRecipe selectRecipe;
    
    private RecipeData currentRecipe;
    private int targetAmount;
    private int smeltedAmount = 0;
    private bool isSmelting = false;
    private Coroutine smeltingCoroutine;

    public void Start()
    {
        playerInventory = InventoryManager.instance.playerInventoryScript;
        furnaceInventory = new FurnaceInventory(5, 3); // Arbitrary slot count — adjust as needed
        building.internalInventory = null; // No longer using InventoryData
        building.gettingRemoved += RemovingFurnace;

        selectRecipe = InventoryManager.instance.FurnaceUI.GetComponent<SelectRecipe>();
    }

    public void InitializeRecipes()
    {
        GameObject furnaceUI = InventoryManager.instance.FurnaceUI;
        GameObject recipeListUI = furnaceUI.transform.Find("Scroll View/Viewport/RecipeList").gameObject;


        foreach (Transform child in recipeListUI.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (RecipeData recipe in recipes)
        {
            GameObject recipeUI = Instantiate(this.recipeUI, recipeListUI.transform);
            
            RecipeButton recipeButton = recipeUI.GetComponent<RecipeButton>();
            recipeButton.Setup(recipe.name, OnRecipeClicked);
            GameObject inputs = recipeUI.transform.Find("Inputs").gameObject;
            GameObject outputs = recipeUI.transform.Find("Outputs").gameObject;

            foreach (RecipeSlotData input in recipe.Input)
            {
                GameObject inputUI = Instantiate(item, inputs.transform);
                ItemUI inputItemUI = inputUI.GetComponent<ItemUI>();
                inputUI.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                inputItemUI.itemCount = input.Amount;
                inputItemUI.itemIcon = input.Item.ItemImage;
                inputItemUI.UpdateItemUI();
            }

            GameObject outputUI = Instantiate(item, outputs.transform);
            ItemUI outputItemUI = outputUI.GetComponent<ItemUI>();
            outputItemUI.itemCount = recipe.Output.Amount;
            outputItemUI.itemIcon = recipe.Output.Item.ItemImage;
            outputItemUI.UpdateItemUI();

        }
    }
    
    public void OnRecipeClicked(string recipeName)
    {
        RecipeData recipe = recipes.Find(r => r.name == recipeName);
        if (recipe != null)
        {
            Debug.Log("Clicked recipe: " + recipe.Output.Item.Name);
            SelectRecipe(recipe);
        }
        else
        {
            Debug.LogWarning("Recipe not found: " + recipeName);
        }
    }

    public void SelectRecipe(RecipeData recipe)
    {
        selectRecipe.selectedRecipe = recipe;
    }

    
    public void StartSmelting(RecipeData recipe, int amount)
    {
        progressBar.gameObject.SetActive(true);
        if (isSmelting || recipe == null || amount <= 0)
        {
            Debug.LogWarning("Furnace: Cannot start smelting.");
            return;
        }

        currentRecipe = recipe;
        targetAmount = amount;
        smeltedAmount = 0;

        furnaceInventory.Clear();

        for (int i = 0; i < currentRecipe.Input.Count; i++)
        {
            var input = currentRecipe.Input[i];
            furnaceInventory.SetInput(i, input.Item.Name, input.Amount * targetAmount);
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
                building.DropItem(new ItemDataID{name = currentRecipe.Output.Item.Name, amount = currentRecipe.Output.Amount});
            });

            yield return new WaitUntil(() => currentItemFinished);

            smeltedAmount++;

            // Consume inputs and produce output
            foreach (var input in currentRecipe.Input)
            {
                furnaceInventory.SubtractFromInput(input.Item.Name, input.Amount);
            }
        }

        isSmelting = false;
        smeltingCoroutine = null;
        progressBar.gameObject.SetActive(false);
    }

    public void OpenFurnace()
    {
        if (!isSmelting)
        {
            InventoryManager.instance.FurnaceUI.SetActive(true);
            selectRecipe.furnace = this;
            InitializeRecipes();
        }
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
            if (!string.IsNullOrEmpty(input.name) && input.amount > 0)
                InventoryManager.instance.TryAddItemToInventoryData(
                    ItemDatabaseInstance.Instance.GetItemByName(input.name), input.amount, building.internalInventory);
        }

        foreach (var output in furnaceInventory.outputSlots)
        {
            if (!string.IsNullOrEmpty(output.name) && output.amount > 0)
                InventoryManager.instance.TryAddItemToInventoryData(
                    ItemDatabaseInstance.Instance.GetItemByName(output.name), output.amount, building.internalInventory);
        }

        furnaceInventory.Clear();
    }
}