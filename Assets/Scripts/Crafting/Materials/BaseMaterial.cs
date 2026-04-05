using UnityEngine;

[System.Serializable]
public class BaseMaterialQuantity
{
    [SerializeField] private BaseMaterial material;
    [SerializeField] private int quantity;

    public BaseMaterial Material => material;
    public int Quantity => quantity;
    public BaseMaterialQuantity(BaseMaterial material, int quantity)
    {
        this.material = material;
        this.quantity = quantity;
    }
    public void AddQuantity(int amount)
    {
        quantity += amount;
    }
    public void RemoveQuantity(int amount)
    {
        quantity = Mathf.Max(0, quantity - amount);
    }
    public bool HasEnoughQuantity(int requiredAmount)
    {
        return quantity >= requiredAmount;
    }
}

// This is for generic base materials like wood, metal, not specific items. These do not have stats
[CreateAssetMenu(fileName = "BaseMaterial", menuName = "Crafting/BaseMaterial")]
public class BaseMaterial : ScriptableObject
{
    [SerializeField] private string materialName;
    [SerializeField] private Sprite materialIcon;
    public string MaterialName => materialName;
    public Sprite MaterialIcon => materialIcon;
}
