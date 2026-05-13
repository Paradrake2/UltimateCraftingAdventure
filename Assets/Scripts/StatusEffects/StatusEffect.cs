using UnityEngine;

/// <summary>
/// Base ScriptableObject for all status effects.
/// Subclass this to define specific behaviours (stat modifiers, taunt, heal-over-time, etc.).
///
/// <para><b>Lifecycle:</b> <see cref="OnApply"/> is called once when the effect starts,
/// <see cref="OnTick"/> every combat frame while active, and <see cref="OnRemove"/> once
/// when the effect expires or is forcibly removed.</para>
///
/// <para><b>Resistance:</b> set <see cref="ResistanceStat"/> to a stat name (e.g.
/// <c>"TauntResistance"</c>) to use a per-effect resistance check. Leave empty to use
/// the global <c>"StatusEffectResistance"</c> stat.</para>
/// </summary>
public abstract class StatusEffect : ScriptableObject
{
    [SerializeField] private string effectName;
    [SerializeField] private Sprite icon;

    [Tooltip("Stat name used for the resistance check against this specific effect. " +
             "Leave empty to use the global 'StatusEffectResistance' stat.")]
    [SerializeField] private string resistanceStat;

    public string EffectName => effectName;
    public Sprite Icon => icon;
    public string ResistanceStat => resistanceStat;

    /// <summary>Called once when this effect is applied to the target.</summary>
    public virtual void OnApply(StatusEffectInstance instance) { }

    /// <summary>Called every combat frame while the effect is active.</summary>
    public virtual void OnTick(StatusEffectInstance instance, float deltaTime) { }

    /// <summary>Called once when the effect expires or is forcibly removed.</summary>
    public virtual void OnRemove(StatusEffectInstance instance) { }
}
