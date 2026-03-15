using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllyArchetype", menuName = "Scriptable Objects/Ally/Archetype")]
public class AllyArchetype : ScriptableObject
{
    [SerializeField] private string archetypeName;
    [SerializeField] private List<AllyRestrictionRule> rules = new List<AllyRestrictionRule>();
    [SerializeField] private Sprite icon;
    [SerializeField] private StatCollection baseStats = new StatCollection();

    public string ArchetypeName => archetypeName;
    public IReadOnlyList<AllyRestrictionRule> Rules => rules;
    public Sprite Icon => icon;
    public StatCollection BaseStats => baseStats;

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
