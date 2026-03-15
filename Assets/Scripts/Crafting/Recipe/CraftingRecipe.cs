using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "CraftingRecipe", menuName = "Scriptable Objects/CraftingRecipe")]
public class CraftingRecipe : ScriptableObject
{
    [SerializeField] private string recipeName;
    [SerializeField] private Sprite recipeIcon;
    [SerializeField] private Equipment baseEquipment;
    [SerializeField] private List<RecipeIngredient> ingredients = new List<RecipeIngredient>();
    public string RecipeName => recipeName;
    public Sprite RecipeIcon => recipeIcon;
    public Equipment BaseEquipment => baseEquipment;
    public IReadOnlyList<RecipeIngredient> Ingredients => ingredients;
}
