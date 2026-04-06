using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllyPreset", menuName = "Scriptable Objects/Ally/Preset")]
public class AllyPreset : ScriptableObject
{
    [Header("Display")]
    [SerializeField] private string displayName;
    [SerializeField] private Sprite iconOverride;
    [SerializeField] private string defaultName;

    [Header("Data")]
    [SerializeField] private AllyArchetype archetype;

    [Tooltip("If set and non-empty, overrides the archetype BaseStats.")]
    [SerializeField] private StatCollection statsOverride = new StatCollection();

    [Tooltip("Optional equipment to attempt to equip on creation.")]
    [SerializeField] private List<Equipment> startingEquipment = new List<Equipment>();

    public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
    public Sprite IconOverride => iconOverride;
    public string DefaultName => string.IsNullOrWhiteSpace(defaultName) ? DisplayName : defaultName;
    public AllyArchetype Archetype => archetype;
    public StatCollection StatsOverride => statsOverride;
    public IReadOnlyList<Equipment> StartingEquipment => startingEquipment;

    public bool TryCreate(string chosenName, out Ally ally, out string failureReason)
    {
        ally = null;
        failureReason = null;

        if (archetype == null)
        {
            failureReason = "Cannot create ally: preset archetype is null.";
            return false;
        }

        string allyName = string.IsNullOrWhiteSpace(chosenName) ? DefaultName : chosenName;

        StatCollection stats;
        if (statsOverride != null && statsOverride.Stats.Count > 0)
        {
            stats = statsOverride.Clone();
        }
        else
        {
            stats = archetype.BaseStats.Clone();
        }

        Sprite icon = iconOverride != null ? iconOverride : archetype.Icon;

        ally = Ally.CreateRuntime(allyName, archetype, icon, stats);
        ally.CombatStats?.SetSkills(archetype.DefaultSkills);

        TryEquipStartingEquipment(ally, startingEquipment);

        if (!ally.ValidateLoadout(out failureReason))
        {
            ally = null;
            return false;
        }

        return true;
    }

    private static void TryEquipStartingEquipment(Ally ally, IReadOnlyList<Equipment> equipment)
    {
        if (ally == null || equipment == null || equipment.Count == 0) return;

        // Important ordering:
        // - Your AllyEquipmentInventory refuses shields unless a weapon is already equipped.
        // - Some archetypes require a shield (RequireShieldRule).
        // So equip weapons first, then shields, then everything else.

        TryEquipByType(ally, equipment, EquipmentType.Weapon);
        TryEquipByType(ally, equipment, EquipmentType.Shield);

        for (int i = 0; i < equipment.Count; i++)
        {
            var item = equipment[i];
            if (item == null) continue;
            if (item.EquipmentType == EquipmentType.Weapon) continue;
            if (item.EquipmentType == EquipmentType.Shield) continue;
            ally.TryEquip(item, out _);
        }
    }

    private static void TryEquipByType(Ally ally, IReadOnlyList<Equipment> equipment, EquipmentType equipmentType)
    {
        for (int i = 0; i < equipment.Count; i++)
        {
            var item = equipment[i];
            if (item == null) continue;
            if (item.EquipmentType != equipmentType) continue;
            ally.TryEquip(item, out _);
        }
    }
}
