using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyLootTable", menuName = "Loot/EnemyLootTable")]
public class EnemyLootTable : ScriptableObject
{
    [SerializeField] private List<ItemQuantity> lootItems = new List<ItemQuantity>();
    public IReadOnlyList<ItemQuantity> LootItems => lootItems;
    [SerializeField] private List<EquipmentLootTableEntry> lootEquipment = new List<EquipmentLootTableEntry>();
    public IReadOnlyList<EquipmentLootTableEntry> LootEquipment => lootEquipment;
    public ItemQuantity GetItemDrop()
    {
        if (lootItems == null || lootItems.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, lootItems.Count);
        return lootItems[randomIndex];
    }

    public Equipment GetEquipmentDrop()
    {
        if (lootEquipment == null || lootEquipment.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, lootEquipment.Count);
        return lootEquipment[randomIndex].equipment;
    }
    public object GetLootDrop()
    {
        bool dropItem = Random.value < 0.33f; // 33% chance to drop an item
        bool dropEquipment = Random.value >= 0.33f && Random.value < 0.66f; // 33% chance to drop equipment
        if (dropItem)
        {
            return GetItemDrop();
        }
        else if (dropEquipment)
        {
            return GetEquipmentDrop();
        }
        else
        {
            return null;
        }
    }
}
