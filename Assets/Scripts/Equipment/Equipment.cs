using UnityEngine;
using System.Collections.Generic;

public enum EquipmentType
{
    Weapon,
    Shield,
    Helmet,
    Chestplate,
    Leggings,
    Boots,
    Gloves,
    Bracers,
    Accessory
}
public enum EquipmentRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Mythical,
    Ultimate,
    Godly
}
[System.Serializable]
public class EquipmentStatModifier
{
    public EquipmentRarity rarity;
    public float GetModifier(EquipmentRarity rarity)
    {
        switch (rarity)
        {
            case EquipmentRarity.Common: return 1f;
            case EquipmentRarity.Uncommon: return 1.5f;
            case EquipmentRarity.Rare: return 2f;
            case EquipmentRarity.Epic: return 3;
            case EquipmentRarity.Legendary: return 5f;
            case EquipmentRarity.Mythical: return 6f;
            case EquipmentRarity.Ultimate: return 7.5f;
            case EquipmentRarity.Godly: return 10f;
            default: return 1f;
        }
    }
    public void SetRarity(EquipmentRarity newRarity)
    {
        rarity = newRarity;
    }
}

[CreateAssetMenu(fileName = "Equipment", menuName = "Scriptable Objects/Equipment")]
public class Equipment : ScriptableObject
{
    [SerializeField] private string equipmentName;
    [SerializeField] private Sprite icon;
    [SerializeField] private StatCollection stats = new StatCollection();
    [SerializeField] private EquipmentType equipmentType;
    [SerializeField] private List<EquipmentTag> tags = new List<EquipmentTag>();
    [SerializeField] private EquipmentStatModifier statModifier = new EquipmentStatModifier();
    [SerializeField] private EquipmentRarity rarity;
    [SerializeField] private EquipmentTag tag;
    [SerializeField] private string id;
    [SerializeField] private List<EquipmentEnchantmentHolder> enchantments = new List<EquipmentEnchantmentHolder>();
    public string EquipmentName => equipmentName;
    public Sprite Icon => icon;
    public StatCollection Stats => stats;
    public EquipmentType EquipmentType => equipmentType;
    public IReadOnlyList<EquipmentTag> Tags => tags;
    public EquipmentRarity Rarity => rarity;
    public EquipmentStatModifier StatModifier => statModifier;
    public EquipmentTag Tag => tag;
    public string ID => id;
    public IReadOnlyList<EquipmentEnchantmentHolder> Enchantments => enchantments;
    public Equipment(string name, Sprite icon, StatCollection stats, EquipmentType equipmentType, EquipmentRarity rarity)
    {
        equipmentName = name;
        this.icon = icon;
        this.stats = stats;
        this.equipmentType = equipmentType;
        this.rarity = rarity;
        ApplyModifier(rarity);
        GenerateID();
    }
    private void OnEnable()
    {
        stats ??= new StatCollection();
        tags ??= new List<EquipmentTag>();
        statModifier ??= new EquipmentStatModifier();
        enchantments ??= new List<EquipmentEnchantmentHolder>();
    }
    public void SetTag(EquipmentTag newTag)
    {
        tag = newTag;
    }
    public void SetStats(StatCollection newStats)
    {
        stats = newStats;
    }
    public float GetStatValue(Stat stat)
    {
        return stats.GetStatValue(stat);
    }
    public float GetStatValue(string statName)
    {
        return stats.GetStatValue(statName);
    }
    public bool HasStat(Stat stat)
    {
        return stats.TryGetStatValue(stat, out _);
    }
    public void SetSprite(Sprite newIcon)
    {
        icon = newIcon;
    }
    public void SetEquipmentName(string newName)
    {
        equipmentName = newName;
    }

    public bool HasTag(EquipmentTag tag)
    {
        if (tag == null || tags == null) return false;
        return tags.Contains(tag);
    }

    public bool HasAnyTag(IEnumerable<EquipmentTag> requiredTags)
    {
        if (requiredTags == null || tags == null) return false;
        foreach (var requiredTag in requiredTags)
        {
            if (requiredTag == null) continue;
            if (tags.Contains(requiredTag)) return true;
        }
        return false;
    }
    private void ApplyModifier(EquipmentRarity rarity)
    {
        float modifier = statModifier.GetModifier(rarity);
        foreach (var statValue in stats.Stats)
        {
            statValue.SetValue(statValue.Value * modifier);
        }
    }
    public void GenerateID()
    {
        id = System.Guid.NewGuid().ToString();
    }
    public void AddTag(EquipmentTag newTag)
    {
        if (newTag != null && !tags.Contains(newTag))
        {
            tags.Add(newTag);
        }
    }
    public void ApplyStatModifier(Stat stat, float modifierAmount)
    {
        float newNum = stats.GetStatValue(stat) * modifierAmount;
        stats.SetStat(stat, newNum);
    }
    public void ApplyEnchantments()
    {
        enchantments ??= new List<EquipmentEnchantmentHolder>();

        foreach (var enchantmentHolder in enchantments)
        {
            if (enchantmentHolder.Enchantment != null && !enchantmentHolder.BeenUsed)
            {
                enchantmentHolder.Enchantment.Apply(this);
                enchantmentHolder.MarkAsUsed();
            }
        }
    }
    public void AddEnchantment(Enchantment enchantment)
    {
        if (enchantment != null)
        {
            enchantments.Add(new EquipmentEnchantmentHolder(enchantment));
        }
    }

}
