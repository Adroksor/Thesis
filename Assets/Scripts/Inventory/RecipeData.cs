using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "InventorySystem/RecipeData")]
public class RecipeData : ScriptableObject
{
    public int ID => GetInstanceID();
    [field: SerializeField]
    public string recipeName { get; set; }
    [field: SerializeField]
    public ItemStack Output { get; set; }
    [field: SerializeField]
    public List<ItemStack> Input { get; set; }
}