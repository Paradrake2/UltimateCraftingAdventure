using UnityEngine;

[CreateAssetMenu(fileName = "RecipeItemSlot", menuName = "Recipes/RecipeItemSlot")]
public class RecipeItemSlot : ScriptableObject
{
    [SerializeField] private MaterialTag[] materialTag;
    [SerializeField] private int quantity = 1;
    public MaterialTag[] MaterialTag => materialTag;
    public int Quantity => quantity;
    public bool IsValid(MaterialTag tag)
    {
        foreach (var t in materialTag)
        {
            if (t == tag)
                return true;
        }
        return false;
    }
}
