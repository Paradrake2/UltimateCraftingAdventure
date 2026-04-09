using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemInventory", menuName = "Inventory/Item Inventory")]
public class ItemInventory : ScriptableObject
{
    [SerializeField] private List<ItemQuantity> ownedItems = new List<ItemQuantity>();
    [System.NonSerialized] private List<ItemQuantity> runtimeItems;
    public IReadOnlyList<ItemQuantity> OwnedItems => runtimeItems;
    private static ItemInventory instance;
    public static ItemInventory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<ItemInventory>("ItemInventory");
                if (instance == null)
                {
                    Debug.LogError("ItemInventory asset not found in Resources folder. Please create an ItemInventory asset and place it in the Resources folder.");
                }
            }
            return instance;
        }
    }
    private void OnEnable()
    {
        runtimeItems = new List<ItemQuantity>(ownedItems ?? new List<ItemQuantity>());
    }

    public void AddItem(Item item, int quantity)
    {
        ItemQuantity existingItem = runtimeItems.Find(i => i.Item == item);
        if (existingItem != null)
        {
            existingItem.AddQuantity(quantity);
        }
        else
        {
            runtimeItems.Add(new ItemQuantity(item, quantity));
        }
    }
    public void RemoveItem(Item item, int quantity)
    {
        ItemQuantity existingItem = runtimeItems.Find(i => i.Item == item);
        if (existingItem != null)
        {
            existingItem.RemoveQuantity(quantity);
            if (existingItem.Quantity <= 0)
            {
                runtimeItems.Remove(existingItem);
            }
        }
    }
    public bool HasItem(Item item, int requiredQuantity)
    {
        ItemQuantity existingItem = runtimeItems.Find(i => i.Item == item);
        return existingItem != null && existingItem.HasEnoughQuantity(requiredQuantity);
    }

    public void Clear()
    {
        runtimeItems.Clear();
    }
}
