using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FurnaceUI : MonoBehaviour
{
    public Furnace furnace;
    public GameObject item;
    public GameObject recipeUIp;
    public Button smeltButton;

    public SelectRecipe selectRecipe;


    private void Start()
    {
        selectRecipe = InventoryManager.instance.FurnaceUI.GetComponent<SelectRecipe>();

    }

    private void OnEnable()
    {
        selectRecipe.amountSelection.onAmountChanged += UpdateAllRecipeUIs;
    }

    private void OnDisable()
    {
        selectRecipe.amountSelection.onAmountChanged -= UpdateAllRecipeUIs;
    }

    public void InitializeRecipes()
    {
        GameObject furnaceUI = InventoryManager.instance.FurnaceUI;
        GameObject recipeListUI = furnaceUI.transform.Find("Scroll View/Viewport/RecipeList").gameObject;


        foreach (Transform child in recipeListUI.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (RecipeData recipe in InventoryManager.instance.furnaceRecipes)
        {
            List<ItemDataID> items = new List<ItemDataID>();
            
            GameObject recipeUI = Instantiate(recipeUIp, recipeListUI.transform);
            
            RecipeButton recipeButton = recipeUI.GetComponent<RecipeButton>();
            recipeButton.Setup(recipe.name, OnRecipeClicked);
            GameObject inputs = recipeUI.transform.Find("Inputs").gameObject;
            GameObject outputs = recipeUI.transform.Find("Outputs").gameObject;

            foreach (RecipeSlotData input in recipe.Input)
            {
                GameObject inputUI = Instantiate(item, inputs.transform);
                ItemUI inputItemUI = inputUI.GetComponent<ItemUI>();
                inputUI.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                inputItemUI.itemCount = input.Amount * selectRecipe.amountSelection.amount;
                inputItemUI.itemIcon = input.Item.ItemImage;
                inputItemUI.UpdateItemUI();

                ItemDataID itemData = new ItemDataID
                {
                    name = input.Item.Name,
                    amount = input.Amount
                };
                items.Add(itemData);
            }
            bool canSmelt =
                InventoryManager.instance.DoesInventoryHaveItems(
                    InventoryManager.instance.playerInventory.inventoryData, items);
            
            recipeButton.button.interactable = canSmelt;
            smeltButton.interactable = canSmelt;
            

            GameObject outputUI = Instantiate(item, outputs.transform);
            ItemUI outputItemUI = outputUI.GetComponent<ItemUI>();
            outputItemUI.itemCount = recipe.Output.Amount;
            outputItemUI.itemIcon = recipe.Output.Item.ItemImage;
            outputItemUI.UpdateItemUI();
        }
    }
    
    public void UpdateAllRecipeUIs(int selectedAmount)
{
    GameObject furnaceUI = InventoryManager.instance.FurnaceUI;
    GameObject recipeListUI = furnaceUI.transform.Find("Scroll View/Viewport/RecipeList").gameObject;

    foreach (Transform recipeUI in recipeListUI.transform)
    {
        RecipeButton recipeButton = recipeUI.GetComponent<RecipeButton>();
        if (recipeButton == null || string.IsNullOrEmpty(recipeButton.recipeName)) continue;

        RecipeData recipe = InventoryManager.instance.furnaceRecipes.Find(r => r.name == recipeButton.recipeName);
        if (recipe == null) continue;

        // Update inputs
        Transform inputs = recipeUI.Find("Inputs");
        if (inputs != null)
        {
            for (int i = 0; i < recipe.Input.Count && i < inputs.childCount; i++)
            {
                Transform inputChild = inputs.GetChild(i);
                if (inputChild.TryGetComponent(out ItemUI inputUI))
                {
                    inputUI.itemCount = recipe.Input[i].Amount * selectedAmount;
                    inputUI.UpdateItemUI();
                }
            }
        }

        // Update outputs
        Transform outputs = recipeUI.Find("Outputs");
        if (outputs != null && outputs.childCount > 0)
        {
            Transform outputChild = outputs.GetChild(0);
            if (outputChild.TryGetComponent(out ItemUI outputUI))
            {
                outputUI.itemCount = recipe.Output.Amount * selectedAmount;
                outputUI.UpdateItemUI();
            }
        }

        // Update interactability based on required input
        List<ItemDataID> requiredItems = recipe.Input.Select(input => new ItemDataID
        {
            name = input.Item.Name,
            amount = input.Amount * selectedAmount
        }).ToList();

        bool canSmelt = InventoryManager.instance.DoesInventoryHaveItems(
            InventoryManager.instance.playerInventory.inventoryData, requiredItems);

        recipeButton.button.interactable = canSmelt;
    }

    UpdateSmeltButtonInteractability();
}
    
    public void UpdateSmeltButtonInteractability()
    {
        RecipeData recipe = selectRecipe.selectedRecipe;
        int amount = selectRecipe.amountSelection.amount;

        if (recipe == null || amount <= 0)
        {
            smeltButton.interactable = false;
            return;
        }

        List<ItemDataID> totalNeededItems = new List<ItemDataID>();

        foreach (RecipeSlotData input in recipe.Input)
        {
            totalNeededItems.Add(new ItemDataID
            {
                name = input.Item.Name,
                amount = input.Amount * amount
            });
        }

        bool canSmelt = InventoryManager.instance.DoesInventoryHaveItems(
            InventoryManager.instance.playerInventory.inventoryData,
            totalNeededItems
        );

        smeltButton.interactable = canSmelt;
    }


    
    public void OnRecipeClicked(string recipeName)
    {
        RecipeData recipe = InventoryManager.instance.furnaceRecipes.Find(r => r.name == recipeName);
        if (recipe != null)
        {
            Debug.Log("Clicked recipe: " + recipe.Output.Item.Name);
            selectRecipe.selectedRecipe = recipe;

            Transform iconTransform = transform.Find("AmountSelection/ItemIcon");
            iconTransform.gameObject.SetActive(true);
            if (iconTransform != null)
            {
                Image icon = iconTransform.gameObject.GetComponent<Image>();
                if (icon != null)
                {
                    icon.sprite = recipe.Output.Item.ItemImage;
                }
                else
                {
                    Debug.LogError("Image component missing on ItemIcon.");
                }
            }
        }
        UpdateSmeltButtonInteractability();

    }
}
