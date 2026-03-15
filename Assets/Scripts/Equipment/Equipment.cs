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
    Accessory
}

[CreateAssetMenu(fileName = "Equipment", menuName = "Scriptable Objects/Equipment")]
public class Equipment : ScriptableObject
{
    [SerializeField] private string equipmentName;
    [SerializeField] private Sprite icon;
    [SerializeField] private StatCollection stats = new StatCollection();
    [SerializeField] private EquipmentType equipmentType;
    [SerializeField] private List<EquipmentTag> tags = new List<EquipmentTag>();
    public string EquipmentName => equipmentName;
    public Sprite Icon => icon;
    public StatCollection Stats => stats;
    public EquipmentType EquipmentType => equipmentType;
    public IReadOnlyList<EquipmentTag> Tags => tags;
    public Equipment(string name, Sprite icon, StatCollection stats, EquipmentType equipmentType)
    {
        this.equipmentName = name;
        this.icon = icon;
        this.stats = stats;
        this.equipmentType = equipmentType;
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
}
