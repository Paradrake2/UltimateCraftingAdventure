using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScrapTier
{
    [SerializeField] private int minLevel;
    [SerializeField] private int maxLevel;
    [SerializeField] private List<BaseMaterial> materials = new List<BaseMaterial>();

    public bool ContainsLevel(int level) => level >= minLevel && level <= maxLevel;

    // Quantity resets at the start of each tier: level 12 in an 11-20 tier → 2, level 18 → 8
    public List<BaseMaterialQuantity> GetResult(int level)
    {
        int quantity = (level - minLevel) + 1;
        var results = new List<BaseMaterialQuantity>();
        foreach (var material in materials)
        {
            results.Add(new BaseMaterialQuantity(material, quantity));
        }
        return results;
    }
}

[System.Serializable]
public class RarityBonus
{
    [SerializeField] private EquipmentRarity rarity;
    [SerializeField] private List<BaseMaterialQuantity> bonusMaterials = new List<BaseMaterialQuantity>();

    public EquipmentRarity Rarity => rarity;
    public List<BaseMaterialQuantity> BonusMaterials => bonusMaterials;
}

[CreateAssetMenu(fileName = "ScrapperConfig", menuName = "Scrapping/ScrapperConfig")]
public class ScrapperConfig : ScriptableObject
{
    [SerializeField] private List<ScrapTier> tiers = new List<ScrapTier>();
    [SerializeField] private List<RarityBonus> rarityBonuses = new List<RarityBonus>();

    public bool TryGetTier(int level, out ScrapTier tier)
    {
        tier = tiers.Find(t => t.ContainsLevel(level));
        return tier != null;
    }

    public bool TryGetRarityBonus(EquipmentRarity rarity, out RarityBonus bonus)
    {
        bonus = rarityBonuses.Find(r => r.Rarity == rarity);
        return bonus != null;
    }
}
