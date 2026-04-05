using System.Collections.Generic;
using UnityEngine;

public static class ScrapEquipment
{
    public static List<BaseMaterialQuantity> Scrap(Equipment equipment, ScrapperConfig config)
    {
        var results = new List<BaseMaterialQuantity>();

        if (config.TryGetTier(equipment.Level, out ScrapTier tier))
            foreach (var material in tier.GetResult(equipment.Level))
            {
                results.Add(material);
            }

        if (config.TryGetRarityBonus(equipment.Rarity, out RarityBonus bonus))
            results.AddRange(bonus.BonusMaterials);

        return results;
    }
}
