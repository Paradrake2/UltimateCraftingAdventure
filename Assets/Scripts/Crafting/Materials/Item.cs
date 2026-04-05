using UnityEngine;

[System.Serializable]
public class ItemQuantity
{
    [SerializeField] private Item item;
    [SerializeField] private int quantity;
    public Item Item => item;
    public int Quantity => quantity;
    public ItemQuantity(Item item, int quantity)
    {
        this.item = item;
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


// These items are for crafting. They are used in recipes
[CreateAssetMenu(fileName = "Item", menuName = "Crafting/Item")]
public class Item : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite icon;
    [SerializeField] private StatCollection statCol = new StatCollection(); // not sure if this will be used
    public string ItemName => itemName;
    public Sprite Icon => icon;
    public StatCollection StatCol => statCol;
}
