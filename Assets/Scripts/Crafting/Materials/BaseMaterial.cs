using UnityEngine;

// This is for generic base materials like wood, metal, not specific items. These do not have stats

[CreateAssetMenu(fileName = "BaseMaterial", menuName = "Crafting/BaseMaterial")]
public class BaseMaterial : ScriptableObject
{
    [SerializeField] private string materialName;
    [SerializeField] private Sprite materialIcon;
    [SerializeField] private int quantity = 1;
    public string MaterialName => materialName;
    public Sprite MaterialIcon => materialIcon;
    public int Quantity => quantity;
    public void AddQuantity(int amount)
    {
        quantity += amount;
    }
    public bool HasEnoughQuantity(int requiredAmount)
    {
        return quantity >= requiredAmount;
    }
    public void ConsumeQuantity(int amount)
    {
        if (HasEnoughQuantity(amount))
        {
            quantity -= amount;
        }
        else
        {
            Debug.LogWarning($"Not enough {materialName} to consume. Required: {amount}, Available: {quantity}");
        }
    }
}
