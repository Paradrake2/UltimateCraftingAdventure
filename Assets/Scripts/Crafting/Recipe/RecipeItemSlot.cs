using UnityEngine;

[CreateAssetMenu(fileName = "RecipeItemSlot", menuName = "Recipes/RecipeItemSlot")]
public class RecipeItemSlot : ScriptableObject
{
    [SerializeField] private Item item;
    [SerializeField] private int quantity = 1;
    public Item Item => item;
    public int Quantity => quantity;
}
