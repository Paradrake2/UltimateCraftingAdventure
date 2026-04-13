using UnityEngine;

[System.Serializable]
public class LootGenerationModifierChance
{
    public EquipmentGenerationModifier EquipmentGenerationModifier;
    public float Chance;
}

public class LootGeneration : MonoBehaviour
{
    private struct RarityEntry
    {
        public EquipmentRarity Rarity;
        public float MinLuck;           // luck required for this rarity to be eligible
        public float MaxLuck;           // luck at which this rarity is excluded (exclusive); use float.MaxValue for never
        public float BaseWeight;        // weight at MinLuck (fading entries) or base floor (growing entries)
        public float WeightGrowthPerLuck; // only meaningful when MaxLuck = float.MaxValue: extra weight per luck above MinLuck

        public RarityEntry(EquipmentRarity rarity, float minLuck, float maxLuck, float baseWeight, float weightGrowthPerLuck = 0f)
        {
            Rarity = rarity; MinLuck = minLuck; MaxLuck = maxLuck;
            BaseWeight = baseWeight; WeightGrowthPerLuck = weightGrowthPerLuck;
        }

        /// <summary>
        /// Fading entries (finite MaxLuck): weight slides from BaseWeight at MinLuck down to 0 at MaxLuck.
        /// Growing entries (infinite MaxLuck): weight starts at BaseWeight at MinLuck and grows by WeightGrowthPerLuck per luck point.
        /// </summary>
        public float GetEffectiveWeight(float luck)
        {
            if (luck < MinLuck || luck >= MaxLuck) return 0f;

            if (MaxLuck < float.MaxValue / 2f)
                return BaseWeight * (MaxLuck - luck) / (MaxLuck - MinLuck);
            else
                return BaseWeight + (luck - MinLuck) * WeightGrowthPerLuck;
        }
    }

    // Per-rarity luck thresholds, weights, and growth rates.
    // Fading (finite MaxLuck):  weight shrinks continuously as luck rises through the window.
    // Growing (MaxValue MaxLuck): weight grows continuously as luck rises past MinLuck.
    private static readonly RarityEntry[] RarityTable =
    {
        //                                                         baseWeight  growthPerLuck
        new RarityEntry(EquipmentRarity.Common,      0f,  50f,   40f),
        new RarityEntry(EquipmentRarity.Uncommon,    0f,  75f,   30f),
        new RarityEntry(EquipmentRarity.Rare,        0f,  90f,   20f),
        new RarityEntry(EquipmentRarity.Epic,        5f,  95f,   15f),
        new RarityEntry(EquipmentRarity.Legendary,  20f, 100f,   10f),
        new RarityEntry(EquipmentRarity.Mythical,   50f, float.MaxValue,  5f,  0.15f),
        new RarityEntry(EquipmentRarity.Ultimate,   75f, float.MaxValue,  2f,  0.08f),
        new RarityEntry(EquipmentRarity.Godly,      90f, float.MaxValue,  0.5f,0.03f),
    };

    /// <summary>
    /// Rolls an equipment rarity using continuous luck-scaled weights.
    /// Every point of luck shifts the distribution — lower rarities fade out smoothly
    /// and higher rarities grow in, rather than flipping at hard thresholds.
    /// </summary>
    public static EquipmentRarity RollRarity(float luck)
    {
        float total = 0f;
        for (int i = 0; i < RarityTable.Length; i++)
            total += RarityTable[i].GetEffectiveWeight(luck);

        if (total <= 0f) return EquipmentRarity.Godly;

        float roll = Random.Range(0f, total);
        float cumulative = 0f;
        for (int i = 0; i < RarityTable.Length; i++)
        {
            cumulative += RarityTable[i].GetEffectiveWeight(luck);
            if (roll <= cumulative)
                return RarityTable[i].Rarity;
        }
        return EquipmentRarity.Common;
    }

    /// <summary>Returns a generation modifier sampled from the map's configured chances.</summary>
    public static EquipmentGenerationModifier GetGenerationModifier(CombatMap map)
    {
        if (map == null || map.LootGenerationModifierChances == null) return EquipmentGenerationModifier.None;

        float totalChance = 0f;
        float randomValue = Random.value;
        foreach (var entry in map.LootGenerationModifierChances)
        {
            totalChance += entry.Chance;
            if (randomValue <= totalChance)
                return entry.EquipmentGenerationModifier;
        }
        return EquipmentGenerationModifier.None;
    }
}
