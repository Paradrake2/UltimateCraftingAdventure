using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentRecipe", menuName = "Recipes/EquipmentRecipe")]
public class EquipmentRecipe : ScriptableObject
{
    [SerializeField] private string recipeName;
    [SerializeField] private Sprite recipeIcon;
    [SerializeField] private Equipment baseEquipment;
    [SerializeField] private List<BaseMaterial> baseMaterials = new List<BaseMaterial>();
    [SerializeField] private List<RecipeItemSlot> itemSlots = new List<RecipeItemSlot>();
    public string RecipeName => recipeName;
    public Sprite RecipeIcon => recipeIcon;
    public Equipment BaseEquipment => baseEquipment;
    public IReadOnlyList<BaseMaterial> BaseMaterials => baseMaterials;
    public IReadOnlyList<RecipeItemSlot> ItemSlots => itemSlots;
}
