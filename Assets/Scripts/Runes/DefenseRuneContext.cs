using System.Collections.Generic;

/// <summary>
/// Passed to every <see cref="DefenseRune"/> when the owning ally is attacked.
/// Runes may freely modify <see cref="DamageInstances"/>, add retaliation damage
/// via <see cref="RetaliationInstances"/>, or set <see cref="IsCancelled"/> to
/// skip all damage entirely (retaliation is still applied when cancelled).
/// </summary>
public class DefenseRuneContext
{
    /// <summary>
    /// The incoming damage instances. Runes may reduce, replace, or remove
    /// individual entries. To reduce a hit, replace it using
    /// <c>DamageInstances[i] = DamageInstances[i].WithAmount(newAmount)</c>.
    /// </summary>
    public List<DamageInstance> DamageInstances;

    /// <summary>
    /// Damage to be dealt back to <see cref="Attacker"/> after all defense runes
    /// run. Runes add entries here to create reflection or retaliation effects.
    /// </summary>
    public List<DamageInstance> RetaliationInstances;

    /// <summary>The ally receiving the attack.</summary>
    public Ally Defender;

    /// <summary>
    /// The enemy that initiated the attack. May be <c>null</c> for
    /// non-enemy damage sources (e.g., environmental hazards).
    /// </summary>
    public Enemy Attacker;

    /// <summary>
    /// When set to <c>true</c> by a rune, the final damage step is skipped
    /// entirely — even if <see cref="DamageInstances"/> is non-empty.
    /// Retaliation in <see cref="RetaliationInstances"/> is still applied.
    /// </summary>
    public bool IsCancelled;

    public DefenseRuneContext(List<DamageInstance> damageInstances, Ally defender, Enemy attacker)
    {
        DamageInstances      = damageInstances ?? new List<DamageInstance>();
        RetaliationInstances = new List<DamageInstance>();
        Defender             = defender;
        Attacker             = attacker;
        IsCancelled          = false;
    }
}
