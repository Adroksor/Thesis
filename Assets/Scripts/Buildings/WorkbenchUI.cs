using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WorkbenchUI : MonoBehaviour
{
    public Workbench workbench;
    public GameObject item;
    public GameObject recipeUIp;
    public Button craftButton;

    public SelectRecipe selectRecipe;
    
    
    private void Start()
    {
        selectRecipe = InventoryManager.instance.WorkbenchUI.GetComponent<SelectRecipe>();
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
        GameObject workbenchUI = InventoryManager.instance.WorkbenchUI;
        GameObject recipeListUI = workbenchUI.transform.Find("Scroll View/Viewport/RecipeList").gameObject;

        
        foreach (Transform child in recipeListUI.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (RecipeData recipe in ItemDatabaseInstance.instance.workbenchRecipes)
        {
            List<ItemStack> items = new List<ItemStack>();

            GameObject recipeUI = Instantiate(recipeUIp, recipeListUI.transform);

            RecipeButton recipeButton = recipeUI.GetComponent<RecipeButton>();
            recipeButton.Setup(recipe.name, OnRecipeClicked);
            GameObject inputs = recipeUI.transform.Find("Inputs").gameObject;
            GameObject outputs = recipeUI.transform.Find("Outputs").gameObject;

            foreach (ItemStack input in recipe.Input)
            {
                GameObject inputUI = Instantiate(item, inputs.transform);
                ItemUI inputItemUI = inputUI.GetComponent<ItemUI>();
                inputUI.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                inputItemUI.itemCount = input.amount;
                inputItemUI.itemIcon = input.item.ItemImage;
                inputItemUI.UpdateItemUI();

                ItemStack itemData = new ItemStack
                {
                    item = input.item,
                    amount = input.amount
                };
                items.Add(itemData);
            }
            bool canCraft =
                InventoryManager.instance.DoesPlayerHaveItems(items);

            recipeButton.button.interactable = canCraft;
            craftButton.interactable = canCraft;

            GameObject outputUI = Instantiate(item, outputs.transform);
            ItemUI outputItemUI = outputUI.GetComponent<ItemUI>();
            outputItemUI.itemCount = recipe.Output.amount;
            outputItemUI.itemIcon = recipe.Output.item.ItemImage;
            outputItemUI.UpdateItemUI();
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(
                recipeUI.GetComponent<RectTransform>());
        }
    }

    public void UpdateAllRecipeUIs(int selectedAmount)
    {
        GameObject workbenchUI = InventoryManager.instance.WorkbenchUI;
        GameObject recipeListUI = workbenchUI.transform.Find("Scroll View/Viewport/RecipeList").gameObject;

        foreach (Transform recipeUI in recipeListUI.transform)
        {
            RecipeButton recipeButton = recipeUI.GetComponent<RecipeButton>();
            if (recipeButton == null || string.IsNullOrEmpty(recipeButton.recipeName)) continue;

            RecipeData recipe = ItemDatabaseInstance.instance.GetWorkbenchRecipeByname(recipeButton.recipeName);
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
                        inputUI.itemCount = recipe.Input[i].amount * selectedAmount;
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
                    outputUI.itemCount = recipe.Output.amount * selectedAmount;
                    outputUI.UpdateItemUI();
                }
            }
            
            // Update interactability based on required input
            List<ItemStack> requiredItems = recipe.Input.Select(input => new ItemStack
            {
                item = input.item,
                amount = input.amount
            }).ToList();

            bool canSmelt = InventoryManager.instance.DoesPlayerHaveItems(requiredItems);

            recipeButton.button.interactable = canSmelt;
        }
        UpdateCraftButtonInteractability();
    }
    
    public void UpdateCraftButtonInteractability()
    {
        RecipeData recipe = selectRecipe.selectedRecipe;
        int amount = selectRecipe.amountSelection.amount;

        if (recipe == null || amount <= 0)
        {
            craftButton.interactable = false;
            return;
        }

        List<ItemStack> totalNeededItems = new List<ItemStack>();

        foreach (ItemStack input in recipe.Input)
        {
            totalNeededItems.Add(new ItemStack
            {
                item = input.item,
                amount = input.amount * amount
            });
        }

        bool canCraft = InventoryManager.instance.DoesPlayerHaveItems(
            totalNeededItems
        );
        craftButton.interactable = canCraft;
    }

    public void OnRecipeClicked(string recipeName)
    {
        RecipeData recipe = ItemDatabaseInstance.instance.GetWorkbenchRecipeByname(recipeName);
        if (recipe != null)
        {
            Debug.Log("Clicked recipe: " + recipe.Output.item.Name);
            selectRecipe.selectedRecipe = recipe;

            Transform iconTransform = transform.Find("AmountSelection/ItemIcon");
            iconTransform.gameObject.SetActive(true);
            if (iconTransform != null)
            {
                Image icon = iconTransform.gameObject.GetComponent<Image>();
                if (icon != null)
                {
                    icon.sprite = recipe.Output.item.ItemImage;
                }
                else
                {
                    Debug.LogError("Image component missing on ItemIcon.");
                }
            }
        }
        UpdateCraftButtonInteractability();

    }
}
