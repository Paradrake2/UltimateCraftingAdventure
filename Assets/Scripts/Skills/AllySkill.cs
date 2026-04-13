using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all ally skills. Create a ScriptableObject subclass and implement
/// <see cref="Execute"/> to define what the skill does.
/// </summary>
public abstract class AllySkill : ScriptableObject
{
    [SerializeField] private float cooldown = 5f;

    [Tooltip("Tags used by archetype restrictions to allow or forbid this skill.")]
    [SerializeField] private List<SkillTag> tags = new List<SkillTag>();

    [Tooltip("If true, the AttackSpeed stat reduces this skill's cooldown the same way it reduces the auto-attack interval.")]
    [SerializeField] private bool scalesWithAttackSpeed = false;

    [Tooltip("Damage this skill deals. Multiplier scales weapon base damage. " +
             "Attributes add elemental bonus instances. Leave empty to deal no damage.")]
    [SerializeField] private List<SkillDamageEntry> damageEntries = new List<SkillDamageEntry>();

    public float Cooldown => cooldown;
    public IReadOnlyList<SkillTag> Tags => tags;
    public bool ScalesWithAttackSpeed => scalesWithAttackSpeed;

    /// <summary>
    /// Builds damage instances using the new weapon-based system.
    ///
    /// For each damage entry:
    ///   1. baseDamage  = sum of "[DeliveryType]Damage" stat from both hand-slot weapons.
    ///   2. deliveryDmg = baseDamage * (1 + stat["[DeliveryType]DamageMultiplier"]/100) * entry.Multiplier
    ///   3. per attribute bonus = baseDamage * (stat["[Attr]DamageMultiplier"]/100)
    ///
    /// Override in subclasses for exotic behaviour (% of enemy HP, etc.).
    /// </summary>
    public virtual List<DamageInstance> BuildDamageInstances(Ally caster)
    {
        var instances = new List<DamageInstance>();
        if (caster == null) return instances;

        StatCollection stats = caster.CombatStats?.StatCollection;
        if (stats == null) return instances;

        // Determine the delivery type and base weapon damage from equipped hand slots.
        DamageType deliveryType = DamageType.Physical;
        float baseDamage = 0f;
        GetWeaponDamage(caster, out deliveryType, out baseDamage);

        // If there are no configured damage entries, deal 1x delivery damage with no attributes.
        if (damageEntries == null || damageEntries.Count == 0)
        {
            string multiplierStat = deliveryType.ToString() + "DamageMultiplier";
            float deliveryMultiplier = 1f + stats.GetStatValue(multiplierStat) / 100f;
            double deliveryAmount = baseDamage * deliveryMultiplier;
            if (deliveryAmount > 0d)
                instances.Add(new DamageInstance(deliveryType, deliveryAmount));
            return instances;
        }

        foreach (var entry in damageEntries)
        {
            if (entry == null) continue;

            // 1. Delivery damage
            string multiplierStat2 = deliveryType.ToString() + "DamageMultiplier";
            float mult = 1f + stats.GetStatValue(multiplierStat2) / 100f;
            double deliveryAmt = baseDamage * mult * entry.Multiplier;
            if (deliveryAmt > 0d)
                instances.Add(new DamageInstance(deliveryType, deliveryAmt));

            // 2. Elemental attribute bonuses
            if (entry.Attributes != null)
            {
                foreach (DamageAttribute attr in entry.Attributes)
                {
                    string attrMultStat = attr.ToString() + "DamageMultiplier";
                    float attrMult = stats.GetStatValue(attrMultStat) / 100f;
                    if (attrMult <= 0f) continue;
                    double attrAmount = baseDamage * attrMult;
                    instances.Add(new DamageInstance(attr, attrAmount));
                }
            }
        }

        return instances;
    }

    /// <summary>
    /// Legacy overload — used internally. Prefer <see cref="BuildDamageInstances(Ally)"/>.
    /// </summary>
    public List<DamageInstance> BuildDamageInstances(StatCollection casterStats)
    {
        // Without an Ally reference we cannot read weapon slots, so fall back to stat-based lookup.
        var instances = new List<DamageInstance>();
        if (damageEntries == null || casterStats == null) return instances;

        foreach (var entry in damageEntries)
        {
            if (entry == null) continue;
            foreach (DamageType type in System.Enum.GetValues(typeof(DamageType)))
            {
                float baseDmg = casterStats.GetStatValue(type.ToString() + "Damage");
                if (baseDmg <= 0f) continue;
                float mult = 1f + casterStats.GetStatValue(type.ToString() + "DamageMultiplier") / 100f;
                instances.Add(new DamageInstance(type, baseDmg * mult * entry.Multiplier));
            }
        }
        return instances;
    }

    public float GetEffectiveCooldown(StatCollection stats)
    {
        if (!scalesWithAttackSpeed || stats == null) return cooldown;
        float attackSpeed = stats.GetStatValue("AttackSpeed");
        if (attackSpeed <= 0f) return cooldown;
        return Mathf.Max(cooldown / attackSpeed, 0.01f);
    }

    public abstract void Execute(SkillContext context);

    // ---------------------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------------------

    private static void GetWeaponDamage(Ally caster, out DamageType deliveryType, out float baseDamage)
    {
        deliveryType = DamageType.Physical;
        baseDamage = 0f;

        var inv = caster.EquipmentInventory;
        if (inv == null) return;

        // Read both hand slots; the first non-null weapon wins for delivery type,
        // and damage values from both slots are summed.
        ReadWeaponSlot(inv.HandSlot1, ref deliveryType, ref baseDamage);
        ReadWeaponSlot(inv.HandSlot2, ref deliveryType, ref baseDamage);

        // If no weapon damage stats exist on the weapon, fall back to the caster's stat collection.
        if (baseDamage <= 0f)
        {
            StatCollection stats = caster.CombatStats?.StatCollection;
            if (stats != null)
            {
                baseDamage = stats.GetStatValue(deliveryType.ToString() + "Damage");
                if (baseDamage <= 0f)
                    baseDamage = stats.GetStatValue("Damage");
            }
        }
    }

    private static void ReadWeaponSlot(Equipment slot, ref DamageType deliveryType, ref float baseDamage)
    {
        if (slot == null || slot.EquipmentType != EquipmentType.Weapon) return;
        deliveryType = slot.WeaponDamageType;
        string statName = deliveryType.ToString() + "Damage";
        float typed = slot.BaseStats.GetStatValue(statName) + slot.Stats.GetStatValue(statName);
        if (typed > 0f)
            baseDamage += typed;
        else
            baseDamage += slot.BaseStats.GetStatValue("Damage") + slot.Stats.GetStatValue("Damage");
    }
}
