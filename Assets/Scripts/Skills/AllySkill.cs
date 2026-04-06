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

    [Tooltip("If true, the AttackSpeed stat reduces this skill's cooldown the same way it reduced the old auto-attack interval.")]
    [SerializeField] private bool scalesWithAttackSpeed = false;

    [Tooltip("Damage this skill deals. Each entry specifies a damage type and a multiplier applied to the caster's matching damage stat. " +
             "Leave empty to deal no damage (e.g. for healing or buff skills).")]
    [SerializeField] private List<SkillDamageEntry> damageEntries = new List<SkillDamageEntry>();

    /// <summary>Time in seconds between uses of this skill (unmodified by stats).</summary>
    public float Cooldown => cooldown;

    /// <summary>Tags that describe this skill, matched against an archetype's allowed skill tags.</summary>
    public IReadOnlyList<SkillTag> Tags => tags;

    /// <summary>Whether this skill's cooldown is divided by the caster's AttackSpeed stat.</summary>
    public bool ScalesWithAttackSpeed => scalesWithAttackSpeed;

    /// <summary>
    /// Builds the list of <see cref="DamageInstance"/>s this skill will deal, based on the
    /// inspector-configured <see cref="SkillDamageEntry"/> list and the caster's stat collection.
    /// Override this in a subclass for exotic damage behaviour (e.g. % of enemy max health).
    /// </summary>
    public virtual List<DamageInstance> BuildDamageInstances(StatCollection casterStats)
    {
        var instances = new List<DamageInstance>();
        if (damageEntries == null || damageEntries.Count == 0 || casterStats == null)
            return instances;

        foreach (var entry in damageEntries)
        {
            if (entry == null) continue;

            // Base damage from the caster's per-type damage stat
            string damageStat = entry.DamageType.ToString() + "Damage";
            float baseDamage = casterStats.GetStatValue(damageStat);

            // Apply the caster's per-type damage multiplier stat
            string multiplierStat = entry.DamageType.ToString() + "DamageMultiplier";
            float statMultiplier = 1f + casterStats.GetStatValue(multiplierStat) / 100f;

            double amount = baseDamage * statMultiplier * entry.Multiplier;
            if (amount <= 0d) continue;

            instances.Add(new DamageInstance(entry.DamageType, amount));
        }

        return instances;
    }

    /// <summary>
    /// Returns the effective cooldown after applying the AttackSpeed stat (if <see cref="ScalesWithAttackSpeed"/> is true).
    /// </summary>
    public float GetEffectiveCooldown(StatCollection stats)
    {
        if (!scalesWithAttackSpeed || stats == null) return cooldown;
        float attackSpeed = stats.GetStatValue("AttackSpeed");
        if (attackSpeed <= 0f) return cooldown;
        return Mathf.Max(cooldown / attackSpeed, 0.01f);
    }

    /// <summary>
    /// Called each time this skill fires. Implement your skill logic here.
    /// Use <paramref name="context"/> to access the caster, all living allies, and active enemies.
    /// </summary>
    public abstract void Execute(SkillContext context);
}
