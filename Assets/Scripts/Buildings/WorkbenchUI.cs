using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorkbenchUI : MonoBehaviour
{


    public GameObject item;
    public GameObject recipeUIp;



    public void InitializeRecipes()
    {
        GameObject recipeListUI = transform.Find("Scroll View/Viewport/RecipeList").gameObject;

        foreach (Transform child in recipeListUI.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (RecipeData recipe in InventoryManager.instance.workbenchRecipes)
        {
            List<ItemStack> items = new List<ItemStack>();

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
                inputItemUI.itemCount = input.Amount;
                inputItemUI.itemIcon = input.Item.ItemImage;
                inputItemUI.UpdateItemUI();

                ItemStack itemData = new ItemStack
                {
                    item = input.Item,
                    amount = input.Amount
                };
                items.Add(itemData);
            }

            bool canCraft =
                InventoryManager.instance.DoesInventoryHaveItems(
                    InventoryManager.instance.playerInventory.inventoryData, items);

            recipeButton.button.interactable = canCraft;

            GameObject outputUI = Instantiate(item, outputs.transform);
            ItemUI outputItemUI = outputUI.GetComponent<ItemUI>();
            outputItemUI.itemCount = recipe.Output.Amount;
            outputItemUI.itemIcon = recipe.Output.Item.ItemImage;
            outputItemUI.UpdateItemUI();
        }
    }

    public void UpdateAllRecipeUIs()
    {
        GameObject recipeListUI = transform.Find("Scroll View/Viewport/RecipeList").gameObject;

        foreach (Transform recipeUI in recipeListUI.transform)
        {
            RecipeButton recipeButton = recipeUI.GetComponent<RecipeButton>();
            if (recipeButton == null || string.IsNullOrEmpty(recipeButton.recipeName)) continue;

            RecipeData recipe = InventoryManager.instance.workbenchRecipes.Find(r => r.name == recipeButton.recipeName);
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
                        inputUI.itemCount = recipe.Input[i].Amount;
                        inputUI.UpdateItemUI();
                    }
                }
            }
            
            // Update interactability based on required input
            List<ItemStack> requiredItems = recipe.Input.Select(input => new ItemStack
            {
                item = input.Item,
                amount = input.Amount
            }).ToList();

            bool canSmelt = InventoryManager.instance.DoesInventoryHaveItems(
                InventoryManager.instance.playerInventory.inventoryData, requiredItems);

            recipeButton.button.interactable = canSmelt;
        }
    }

    public void OnRecipeClicked(string recipeName)
    {
        RecipeData recipe = InventoryManager.instance.workbenchRecipes.Find(r => r.name == recipeName);
        if (recipe != null)
        {
            foreach (RecipeSlotData slot in recipe.Input)
            {
                InventoryManager.instance.TryRemoveItemsFromInventoryData(slot.Item, slot.Amount, InventoryManager.instance.playerInventory.inventoryData );
            }
            InventoryManager.instance.TryAddItemToInventoryData(recipe.Output.Item, recipe.Output.Amount, InventoryManager.instance.playerInventory.inventoryData);

            UpdateAllRecipeUIs();
        }
    }
}
