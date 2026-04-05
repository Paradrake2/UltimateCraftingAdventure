using UnityEngine;


public enum EquipmentGenerationModifier
{
    None,
    Enchanted,
    Augmented,
    Reinforced,
    Cursed,
    Blessed
}

public class EquipmentStatGenerationNumber
{
    public int GetNumberOfStats(EquipmentRarity rarity)
    {
        switch (rarity)
        {
            case EquipmentRarity.Common: return 1;
            case EquipmentRarity.Uncommon: return 2;
            case EquipmentRarity.Rare: return 3;
            case EquipmentRarity.Epic: return 4;
            case EquipmentRarity.Legendary: return 5;
            case EquipmentRarity.Mythical: return 6;
            case EquipmentRarity.Ultimate: return 7;
            case EquipmentRarity.Godly: return 8;
            default: return 1;
        }
    }
}

public static class EquipmentFactory
{
    // overload for creating equipment without a base, generates random stats based on level, generation modifier and rarity
    // this is used for rewards and loot drops
    public static Equipment GetLootEquipment(Equipment baseEquipment, int level, EquipmentRarity rarity, EquipmentGenerationModifier modifier)
    {
        Equipment newEquipment = ScriptableObject.Instantiate(baseEquipment);
        newEquipment.SetLevel(level);
        newEquipment.SetGenerationModifier(modifier);
        newEquipment.GenerateID();
        GenerateStats(newEquipment, level, rarity);
        ApplyModifier(newEquipment, modifier);
        return newEquipment;
    }

    public static Equipment GetCraftedEquipment(EquipmentRecipe recipe, int level, EquipmentRarity rarity)
    {
        Equipment craftedEquipment = ScriptableObject.Instantiate(recipe.BaseEquipment);
        craftedEquipment.SetLevel(level);
        craftedEquipment.SetRarity(rarity);
        craftedEquipment.GenerateID();
        GenerateStats(craftedEquipment, level, rarity);
        return craftedEquipment;
    }
    
    private static void GenerateStats(Equipment equipment, int level, EquipmentRarity rarity)
    {
        int numStats = new EquipmentStatGenerationNumber().GetNumberOfStats(rarity);
        for (int i = 0; i < numStats; i++)
        {
            Stat stat = StatDatabase.Instance.GetRandomStat(); // no check for duplicates is intentional
            float baseValue = stat.BaseValue;
            float modifier = new EquipmentStatModifier().GetModifier(rarity);
            float randomVariation = GetRandomVariation(baseValue);
            float finalValue = (baseValue + randomVariation) * modifier * LevelModifier(1, level);
            equipment.Stats.AddStatValue(stat, finalValue); // add stat to equipment, is AddStatValue in case the random stat is the same as a previous one
        }
    }
    private static float GetRandomVariation(float baseValue)
    {
        float variation = baseValue * 0.25f; // 25% variation
        return baseValue * Random.Range(-variation, variation);
    }
    private static float LevelModifier(float value, int level)
    {
        return value * (1 + (level - 1) * 0.05f);
    }
    private static void ApplyModifier(Equipment equipment, EquipmentGenerationModifier modifier)
    {
        switch (modifier)
        {
            case EquipmentGenerationModifier.None:
                // no modifier applied
                break;
            case EquipmentGenerationModifier.Enchanted:
                ApplyEnchantmentModifier(equipment);
                break;
            case EquipmentGenerationModifier.Augmented:
                ApplyAugmentModifier(equipment);
                break;
            case EquipmentGenerationModifier.Reinforced:
                // increase stats by 10%
                ApplyReinforcementModifier(equipment);
                break;
            case EquipmentGenerationModifier.Cursed:
                // decrease stats by 10%
                foreach (var statValue in equipment.Stats.Stats)
                {
                    statValue.SetValue(statValue.Value * 0.9f);
                }
                break;
            case EquipmentGenerationModifier.Blessed:
                // increase stats by 20%
                foreach (var statValue in equipment.Stats.Stats)
                {
                    statValue.SetValue(statValue.Value * 1.2f);
                }
                break;
        }
    }
    private static void ApplyReinforcementModifier(Equipment equipment)
    {
        int numReinforcements = Random.Range(1, equipment.MaxReinforcementLevel); // number of random reinforcements to apply, up to the max reinforcement level
        for (int i = 0; i < numReinforcements; i++)
        {
            equipment.Reinforce();
        }
        equipment.SetReinforcementLevel(numReinforcements); // set the reinforcement level to the number of reinforcements applied
    }
    private static void ApplyEnchantmentModifier(Equipment equipment)
    {
        // get a random enchantment
        // apply it to the equipment
    }
    private static void ApplyAugmentModifier(Equipment equipment)
    {
        // get a random augment
        // apply it to the equipment
    }
}
