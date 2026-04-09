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

// used for equipment generation
[System.Serializable]
public class EquipmentStatModifier
{
    public EquipmentRarity rarity; // added for reference I think, don't really remember but it might be useful for something
    public float GetModifier(EquipmentRarity rarity)
    {
        switch (rarity)
        {
            case EquipmentRarity.Common: return 1f;
            case EquipmentRarity.Uncommon: return 1.25f;
            case EquipmentRarity.Rare: return 1.5f;
            case EquipmentRarity.Epic: return 2f;
            case EquipmentRarity.Legendary: return 2.5f;
            case EquipmentRarity.Mythical: return 3f;
            case EquipmentRarity.Ultimate: return 4f;
            case EquipmentRarity.Godly: return 5f;
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
    [SerializeField] private StatCollection baseStats = new StatCollection(); // stats that this equipment will always have
    [SerializeField] private StatCollection stats = new StatCollection(); // random stats
    [SerializeField] private EquipmentType equipmentType;
    [SerializeField] private List<EquipmentTag> tags = new List<EquipmentTag>();
    [SerializeField] private EquipmentStatModifier statModifier = new EquipmentStatModifier();
    [SerializeField] private EquipmentRarity rarity;
    [SerializeField] private EquipmentTag tag;
    [SerializeField] private string id;
    [SerializeField] private List<EquipmentEnchantmentHolder> enchantments = new List<EquipmentEnchantmentHolder>();
    [SerializeField] private List<EquipmentAugmentHolder> augments = new List<EquipmentAugmentHolder>();
    [SerializeField] private int reinforcementLevel = 0; // increases baseStats by 10% per level
    [SerializeField] private int maxReinforcementLevel = 5;
    [SerializeField] private List<Item> refundItems = new List<Item>();
    [SerializeField] private int maxRefundItems = 5;
    [SerializeField] private int level = 1;
    [SerializeField] private EquipmentGenerationModifier generationModifier = EquipmentGenerationModifier.None;
    public string EquipmentName => equipmentName;
    public Sprite Icon => icon;
    public StatCollection BaseStats => baseStats;
    public StatCollection Stats => stats;
    public EquipmentType EquipmentType => equipmentType;
    public IReadOnlyList<EquipmentTag> Tags => tags;
    public EquipmentRarity Rarity => rarity;
    public EquipmentStatModifier StatModifier => statModifier;
    public EquipmentTag Tag => tag;
    public string ID => id;
    public IReadOnlyList<EquipmentEnchantmentHolder> Enchantments => enchantments;
    public IReadOnlyList<EquipmentAugmentHolder> Augments => augments;
    public int ReinforcementLevel => reinforcementLevel;
    public int MaxReinforcementLevel => maxReinforcementLevel;
    public IReadOnlyList<Item> RefundItems => refundItems;
    public int MaxRefundItems => maxRefundItems;
    public int Level => level; // set on generation, used for scaling stats, cannot be changed after generation
    public EquipmentGenerationModifier GenerationModifier => generationModifier;
    /**
    public Equipment(string name, Sprite icon, StatCollection stats, EquipmentType equipmentType, EquipmentRarity rarity, int level = 1)
    {
        equipmentName = name;
        this.icon = icon;
        this.stats = stats;
        this.equipmentType = equipmentType;
        this.rarity = rarity;
        this.level = level;
    }
    **/
    private void OnEnable()
    {
        stats ??= new StatCollection();
        tags ??= new List<EquipmentTag>();
        statModifier ??= new EquipmentStatModifier();
        enchantments ??= new List<EquipmentEnchantmentHolder>();
        augments ??= new List<EquipmentAugmentHolder>();
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
    public void GenerateID()
    {
        id = System.Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Returns a runtime ScriptableObject clone of this asset with a fresh ID.
    /// Use this instead of equipping the raw asset directly so the instance
    /// can be identified and saved independently.
    /// </summary>
    public Equipment CloneWithID()
    {
        var clone = Instantiate(this);
        clone.GenerateID();
        return clone;
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
    public void AddAugment(Augment augment)
    {
        if (augment != null)
        {
            augments.Add(new EquipmentAugmentHolder(augment));
        }
    }
    public void Reinforce()
    {
        if (reinforcementLevel >= maxReinforcementLevel) return;
        reinforcementLevel++;
        float reinforcementModifier = 1 + (reinforcementLevel * 0.1f);
        foreach (var statValue in baseStats.Stats)
        {
            statValue.SetValue(statValue.Value * reinforcementModifier);
        }
    }
    public void SetReinforcementLevel(int newLevel)
    {
        reinforcementLevel = Mathf.Clamp(newLevel, 0, maxReinforcementLevel);
    }
    public void AddRefundItem(Item item)
    {
        if (item != null)
        {
            refundItems.Add(item);
        }
    }
    // applied on generation
    public void SetLevel(int newLevel)
    {
        level = Mathf.Max(1, newLevel);
    }
    public void SetGenerationModifier(EquipmentGenerationModifier newModifier)
    {
        generationModifier = newModifier;
    }
    public void SetRarity(EquipmentRarity newRarity)
    {
        rarity = newRarity;
    }

    /// <summary>
    /// Restores all core fields from save data on a freshly created instance.
    /// Called by SaveSystem — do not call from other code.
    /// </summary>
    public void RestoreFromSave(
        string savedId,
        string savedName,
        EquipmentType savedType,
        EquipmentRarity savedRarity,
        int savedLevel,
        int savedReinforcementLevel,
        EquipmentGenerationModifier savedGenerationModifier,
        StatCollection savedBaseStats,
        StatCollection savedStats,
        Sprite savedIcon)
    {
        id                 = savedId;
        equipmentName      = savedName;
        equipmentType      = savedType;
        rarity             = savedRarity;
        level              = Mathf.Max(1, savedLevel);
        reinforcementLevel = Mathf.Clamp(savedReinforcementLevel, 0, maxReinforcementLevel);
        generationModifier = savedGenerationModifier;
        baseStats          = savedBaseStats ?? new StatCollection();
        stats              = savedStats ?? new StatCollection();
        icon               = savedIcon;
        tags               ??= new System.Collections.Generic.List<EquipmentTag>();
        enchantments       ??= new System.Collections.Generic.List<EquipmentEnchantmentHolder>();
        augments           ??= new System.Collections.Generic.List<EquipmentAugmentHolder>();
    }

    /// <summary>
    /// Adds a pre-constructed holder directly (used by SaveSystem to preserve beenUsed state).
    /// </summary>
    public void AddEnchantmentHolder(EquipmentEnchantmentHolder holder)
    {
        enchantments ??= new System.Collections.Generic.List<EquipmentEnchantmentHolder>();
        enchantments.Add(holder);
    }
}
