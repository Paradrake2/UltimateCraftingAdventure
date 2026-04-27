using UnityEngine;

/// <summary>
/// Base class for all utility runes. Subclasses override only the event hooks
/// they care about; all hooks are no-ops by default.
///
/// <para><b>ScriptableObject state warning:</b> hooks are called on the shared
/// asset directly. If your rune needs per-combat state (e.g., a one-shot
/// resurrection), be aware that the same asset instance is used for every ally
/// that equips it. Track per-ally runtime state externally if needed.</para>
/// </summary>
public abstract class UtilityRune : Rune
{
    public override RuneType RuneType => RuneType.Utility;

    /// <summary>Called once when the owning ally enters a combat session.</summary>
    public virtual void OnCombatStart(Ally ally) { }

    /// <summary>
    /// Called when the owning ally's health reaches zero, before the death is
    /// finalised. If this method restores <c>ally.CombatStats.CurrentHealth</c>
    /// to a value greater than zero (e.g. resurrection), the combat system will
    /// treat the ally as still alive.
    /// </summary>
    public virtual void OnAllyDied(Ally ally) { }

    /// <summary>Called when any enemy in the current wave is defeated.</summary>
    public virtual void OnEnemyDied(Ally ally, Enemy enemy) { }

    /// <summary>
    /// Called when all enemies in a wave are defeated, before the next wave
    /// spawns.
    /// </summary>
    public virtual void OnWaveCleared(Ally ally) { }

    /// <summary>
    /// Called at the start of each combat session.
    /// Override to reset any per-combat runtime state on this rune.
    /// </summary>
    public virtual void ResetForCombat(Ally ally) { }
}
