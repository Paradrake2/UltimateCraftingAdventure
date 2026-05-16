using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllyArchetype", menuName = "Scriptable Objects/Ally/Archetype")]
public class AllyArchetype : ScriptableObject
{
    [SerializeField] private string archetypeName;
    [SerializeField] private List<AllyRestrictionRule> rules = new List<AllyRestrictionRule>();
    [SerializeField] private Sprite icon;
    [SerializeField] private StatCollection baseStats = new StatCollection();

    [Header("Skills")]
    [Tooltip("Typed skill slots for this archetype. Add one entry per slot type with an optional default skill.")]
    [SerializeField] private List<DefaultSkillEntry> defaultSkillEntries = new List<DefaultSkillEntry>();

    [Tooltip("Skill tags this archetype is allowed to use. Leave empty to allow all skills.")]
    [SerializeField] private List<SkillTag> allowedSkillTags = new List<SkillTag>();

    public string ArchetypeName => archetypeName;
    public IReadOnlyList<AllyRestrictionRule> Rules => rules;
    public Sprite Icon => icon;
    public StatCollection BaseStats => baseStats;
    public IReadOnlyList<DefaultSkillEntry> DefaultSkillEntries => defaultSkillEntries;
    public IReadOnlyList<SkillTag> AllowedSkillTags => allowedSkillTags;

    /// <summary>
    /// Returns true if this archetype permits the given skill.
    /// If <see cref="AllowedSkillTags"/> is empty, all skills are allowed.
    /// Otherwise the skill must have at least one matching tag.
    /// </summary>
    public bool CanEquipSkill(AllySkill skill, out string failureReason)
    {
        failureReason = null;
        if (skill == null) return true;
        if (allowedSkillTags == null || allowedSkillTags.Count == 0) return true;

        if (skill.Tags != null)
        {
            foreach (SkillTag tag in skill.Tags)
            {
                if (allowedSkillTags.Contains(tag)) return true;
            }
        }

        failureReason = $"'{archetypeName}' cannot use skill '{skill.name}': no matching skill tag.";
        return false;
    }

    public bool CanEquip(Ally ally, AllyEquipmentInventory inventory, Equipment equipment, out string failureReason)
    {
        failureReason = null;
        if (rules == null) return true;

        foreach (var rule in rules)
        {
            if (rule == null) continue;
            if (!rule.CanEquip(ally, inventory, equipment, out failureReason))
            {
                if (string.IsNullOrWhiteSpace(failureReason))
                {
                    failureReason = "Cannot equip due to archetype restriction.";
                }
                return false;
            }
        }

        return true;
    }

    public bool CanUnequip(Ally ally, AllyEquipmentInventory inventory, Equipment equipment, out string failureReason)
    {
        failureReason = null;
        if (rules == null) return true;

        foreach (var rule in rules)
        {
            if (rule == null) continue;
            if (!rule.CanUnequip(ally, inventory, equipment, out failureReason))
            {
                if (string.IsNullOrWhiteSpace(failureReason))
                {
                    failureReason = "Cannot unequip due to archetype restriction.";
                }
                return false;
            }
        }

        return true;
    }

    public bool ValidateLoadout(Ally ally, AllyEquipmentInventory inventory, out string failureReason)
    {
        failureReason = null;
        if (rules == null) return true;

        foreach (var rule in rules)
        {
            if (rule == null) continue;
            if (!rule.ValidateLoadout(ally, inventory, out failureReason))
            {
                if (string.IsNullOrWhiteSpace(failureReason))
                {
                    failureReason = "Invalid loadout for this archetype.";
                }
                return false;
            }
        }

        return true;
    }
}
