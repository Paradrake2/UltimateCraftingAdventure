using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyLootTable", menuName = "Loot/EnemyLootTable")]
public class EnemyLootTable : ScriptableObject
{
    [SerializeField] private List<Item> lootItems = new List<Item>();
    public IReadOnlyList<Item> LootItems => lootItems;
    [SerializeField] private List<EquipmentLootTableEntry> lootEquipment = new List<EquipmentLootTableEntry>();
    public IReadOnlyList<EquipmentLootTableEntry> LootEquipment => lootEquipment;
    public Item GetItemDrop()
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

}
