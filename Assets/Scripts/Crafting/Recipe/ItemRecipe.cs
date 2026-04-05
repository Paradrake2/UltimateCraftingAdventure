using System.Collections.Generic;
using UnityEngine;


// this exists, but is not used yet. I'm not sure if it will ever be used, but it's here just in case.
[CreateAssetMenu(fileName = "ItemRecipe", menuName = "Recipes/ItemRecipe")]
public class ItemRecipe : ScriptableObject
{
    [SerializeField] private string recipeName;
    [SerializeField] private Sprite recipeIcon;
    [SerializeField] private Item resultItem;
    [SerializeField] private int resultQuantity = 1;
    [SerializeField] private List<BaseMaterial> baseMaterials = new List<BaseMaterial>();
    [SerializeField] private List<Item> requiredItems = new List<Item>();
    public string RecipeName => recipeName;
    public Sprite RecipeIcon => recipeIcon;
    public Item ResultItem => resultItem;
    public int ResultQuantity => resultQuantity;
    public IReadOnlyList<BaseMaterial> BaseMaterials => baseMaterials;
    public IReadOnlyList<Item> RequiredItems => requiredItems;
}
