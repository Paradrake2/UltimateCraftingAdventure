using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemRecipe", menuName = "Recipes/ItemRecipe")]
public class ItemRecipe : ScriptableObject
{
    [SerializeField] private string recipeName;
    [SerializeField] private Sprite recipeIcon;
    [SerializeField] private Item resultItem;
    [SerializeField] private int resultQuantity = 1;
    [SerializeField] private List<BaseMaterialHolder> baseMaterials = new List<BaseMaterialHolder>();
    [SerializeField] private List<Item> requiredItems = new List<Item>();
    public string RecipeName => recipeName;
    public Sprite RecipeIcon => recipeIcon;
    public Item ResultItem => resultItem;
    public int ResultQuantity => resultQuantity;
    public IReadOnlyList<BaseMaterialHolder> BaseMaterials => baseMaterials;
    public IReadOnlyList<Item> RequiredItems => requiredItems;
}
