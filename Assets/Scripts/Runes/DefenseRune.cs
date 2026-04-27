using UnityEngine;

/// <summary>
/// Base class for all defensive runes. Subclasses implement
/// <see cref="ApplyEffect"/> to modify incoming damage, add retaliation,
/// or cancel damage entirely via the <see cref="DefenseRuneContext"/>.
///
/// <para><b>ScriptableObject state warning:</b> <see cref="ApplyEffect"/> and
/// <see cref="ResetForCombat"/> are called on the shared asset directly.
/// If your rune needs per-combat instance state (e.g., a one-shot effect),
/// be aware that the same asset instance is used for every ally that equips it.
/// Track per-ally runtime state externally if that distinction matters.</para>
/// </summary>
public abstract class DefenseRune : Rune
{
    public override RuneType RuneType => RuneType.Defense;

    /// <summary>
    /// Called once per defense rune slot when the owning ally is attacked,
    /// before damage is applied to the ally.
    /// Modify <paramref name="ctx"/> to alter or negate the incoming hit.
    /// </summary>
    public abstract void ApplyEffect(DefenseRuneContext ctx);

    /// <summary>
    /// Called at the start of each combat session.
    /// Override to reset any per-combat runtime state on this rune.
    /// </summary>
    public virtual void ResetForCombat() { }
}
