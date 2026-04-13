using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines how a skill deals damage.
/// The delivery type (Physical/Ranged/Magic/Poison) is inherited from the caster's equipped weapons.
/// Elemental attributes add bonus instances on top of the delivery damage.
///
/// Formula:
///   baseDamage        = sum of weapon "[DeliveryType]Damage" stats
///   deliveryDamage    = baseDamage * (1 + stat["[DeliveryType]DamageMultiplier"] / 100) * Multiplier
///   per attribute     = baseDamage * (stat["[Attr]DamageMultiplier"] / 100)  →  one DamageInstance each
/// </summary>
[Serializable]
public class SkillDamageEntry
{
    [Tooltip("Multiplier applied to base weapon damage. 1.0 = 100% of weapon damage, 0.5 = 50%, 1.5 = 150%.")]
    [SerializeField] private float multiplier = 1f;

    [Tooltip("Elemental attributes this skill carries. Each attribute adds bonus damage equal to " +
             "baseDamage * (stat[AttributeDamageMultiplier] / 100).")]
    [SerializeField] private List<DamageAttribute> attributes = new List<DamageAttribute>();

    public float Multiplier => multiplier;
    public IReadOnlyList<DamageAttribute> Attributes => attributes;
}
