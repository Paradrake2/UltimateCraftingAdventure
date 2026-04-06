using System;
using UnityEngine;

/// <summary>
/// One damage component of a skill. The final damage dealt is:
/// <c>casterBaseDamage * multiplier</c>
/// where <c>casterBaseDamage</c> is the sum of the caster's relevant stat and its multiplier stat.
/// </summary>
[Serializable]
public class SkillDamageEntry
{
    [Tooltip("The damage type this entry deals (e.g. Fire, Cold, Physical).")]
    [SerializeField] private DamageType damageType;

    [Tooltip("Multiplier applied to the caster's base damage stat for this type. " +
             "1.0 = 100% of the caster's relevant damage stat. " +
             "Use values < 1 for weaker hits, > 1 for stronger hits.")]
    [SerializeField] private float multiplier = 1f;

    public DamageType DamageType => damageType;
    public float Multiplier => multiplier;
}
