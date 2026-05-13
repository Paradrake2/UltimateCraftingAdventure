/// <summary>
/// Runtime handle for a single active status effect on a target.
/// Holds the definition, remaining duration, the inflictor, and the manager managing it.
/// Effects read and write through this to keep per-instance state separate from the
/// shared ScriptableObject definition.
/// </summary>
public class StatusEffectInstance
{
    /// <summary>The ScriptableObject that defines this effect's behaviour.</summary>
    public StatusEffect Definition { get; }

    /// <summary>Seconds remaining before the effect expires.</summary>
    public float RemainingDuration { get; set; }

    /// <summary>
    /// The entity that applied this effect (e.g. the ally who cast the skill).
    /// May be null if the source is not an <see cref="IStatusEffectTarget"/> (e.g. a trap).
    /// </summary>
    public IStatusEffectTarget Inflictor { get; }

    /// <summary>The manager that owns this instance, i.e. the affected entity's manager.</summary>
    public StatusEffectManager Manager { get; }

    public StatusEffectInstance(
        StatusEffect definition,
        float duration,
        IStatusEffectTarget inflictor,
        StatusEffectManager manager)
    {
        Definition = definition;
        RemainingDuration = duration;
        Inflictor = inflictor;
        Manager = manager;
    }
}
