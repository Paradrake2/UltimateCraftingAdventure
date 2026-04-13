/// <summary>
/// How damage is delivered. Determined by the weapon equipped.
/// Equipment with "[Type]Damage" stats contributes to this pool.
/// Equipment with "[Type]DamageMultiplier" stats scales this pool by a %.
/// </summary>
public enum DamageType
{
    Physical,
    Ranged,
    Magic,
    Poison,
}

/// <summary>
/// Elemental flavour of an attack, declared on the skill.
/// Each attribute adds bonus damage equal to baseDamage * (stat["[Attr]DamageMultiplier"] / 100).
/// Equipment with "[Attr]Resistance" stats reduce this component.
/// </summary>
public enum DamageAttribute
{
    Fire,
    Water,
    Wind,
    Earth,
    Light,
    Darkness,
}
