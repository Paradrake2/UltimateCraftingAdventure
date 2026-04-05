using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseMaterialInventory", menuName = "Inventory/Base Material Inventory")]
public class BaseMaterialInventory : ScriptableObject
{
    [SerializeField] private List<BaseMaterialQuantity> ownedMaterials = new List<BaseMaterialQuantity>();
    public IReadOnlyList<BaseMaterialQuantity> OwnedMaterials => ownedMaterials;

    public void AddMaterial(BaseMaterial material, int quantity)
    {
        BaseMaterialQuantity existing = ownedMaterials.Find(m => m.Material == material);
        if (existing != null)
        {
            existing.AddQuantity(quantity);
        }
        else
        {
            ownedMaterials.Add(new BaseMaterialQuantity(material, quantity));
        }
    }

    public void RemoveMaterial(BaseMaterial material, int quantity)
    {
        BaseMaterialQuantity existing = ownedMaterials.Find(m => m.Material == material);
        if (existing != null)
        {
            existing.RemoveQuantity(quantity);
            if (existing.Quantity <= 0)
            {
                ownedMaterials.Remove(existing);
            }
        }
    }

    public bool HasMaterial(BaseMaterial material, int requiredQuantity)
    {
        BaseMaterialQuantity existing = ownedMaterials.Find(m => m.Material == material);
        return existing != null && existing.HasEnoughQuantity(requiredQuantity);
    }
}
