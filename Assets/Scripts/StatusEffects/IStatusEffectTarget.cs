/// <summary>
/// Implemented by any entity that can receive status effects (Ally, Enemy, etc.).
/// Exposes the <see cref="StatusEffectManager"/> so any source — skills, runes, items,
/// triggers — can apply effects through a single, uniform entry point.
/// </summary>
public interface IStatusEffectTarget
{
    StatusEffectManager StatusEffects { get; }
}
