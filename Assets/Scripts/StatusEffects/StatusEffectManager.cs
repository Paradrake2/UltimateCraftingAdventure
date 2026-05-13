using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all active <see cref="StatusEffectInstance"/>s on a single entity.
/// Owned by <c>AllyCombat</c> and <c>EnemyCombat</c>; exposed via <see cref="IStatusEffectTarget"/>.
///
/// <para><b>Applying effects:</b> call <see cref="TryApply"/> from any source (skills, runes,
/// traps, etc.). The manager performs the resistance check and notifies the effect definition.</para>
///
/// <para><b>Ticking:</b> call <see cref="Tick"/> once per combat frame from the entity's tick
/// method. This advances all active timers and removes expired effects.</para>
///
/// <para><b>Resetting:</b> call <see cref="Clear"/> when a combat session ends or when the
/// entity is reset, so no effects bleed into the next encounter.</para>
/// </summary>
public class StatusEffectManager
{
    private const string DefaultResistanceStat = "StatusEffectResistance";

    private readonly List<StatusEffectInstance> _activeEffects = new List<StatusEffectInstance>();
    private readonly StatCollection _stats;

    /// <summary>The entity that owns this manager.</summary>
    public IStatusEffectTarget Owner { get; }

    /// <summary>
    /// When non-null, the owning entity's targeting logic should be forced to attack this target.
    /// Set and cleared by <see cref="StatusEffects.TauntStatusEffect"/>.
    /// </summary>
    public IStatusEffectTarget ForcedAttackTarget { get; set; }

    /// <summary>All currently active effect instances (read-only view).</summary>
    public IReadOnlyList<StatusEffectInstance> ActiveEffects => _activeEffects;

    public StatusEffectManager(IStatusEffectTarget owner, StatCollection stats)
    {
        Owner = owner;
        _stats = stats;
    }

    /// <summary>The target's stat collection, used by stat-modifying effects.</summary>
    public StatCollection Stats => _stats;

    /// <summary>
    /// Attempts to apply <paramref name="effect"/> to this entity.
    /// Performs a resistance check before applying.
    /// </summary>
    /// <param name="effect">The effect definition to apply.</param>
    /// <param name="duration">How long (in seconds) the effect should last.</param>
    /// <param name="inflictor">The entity that caused this effect. May be null.</param>
    /// <returns><c>true</c> if the effect was applied; <c>false</c> if it was resisted.</returns>
    public bool TryApply(StatusEffect effect, float duration, IStatusEffectTarget inflictor = null)
    {
        if (effect == null || duration <= 0f) return false;

        // Resistance check: use a per-effect stat or fall back to the global one.
        string resistStat = string.IsNullOrEmpty(effect.ResistanceStat)
            ? DefaultResistanceStat
            : effect.ResistanceStat;

        float resistance = _stats != null
            ? Mathf.Clamp(_stats.GetStatValue(resistStat), 0f, 100f)
            : 0f;

        if (resistance > 0f && Random.value < resistance / 100f)
            return false;

        var instance = new StatusEffectInstance(effect, duration, inflictor, this);
        _activeEffects.Add(instance);
        effect.OnApply(instance);
        return true;
    }

    /// <summary>
    /// Advances all active effect timers by <paramref name="deltaTime"/> seconds,
    /// ticking each effect and removing any that have expired.
    /// Call this once per combat frame.
    /// </summary>
    public void Tick(float deltaTime)
    {
        for (int i = _activeEffects.Count - 1; i >= 0; i--)
        {
            StatusEffectInstance inst = _activeEffects[i];
            inst.Definition.OnTick(inst, deltaTime);
            inst.RemainingDuration -= deltaTime;
            if (inst.RemainingDuration <= 0f)
            {
                inst.Definition.OnRemove(inst);
                _activeEffects.RemoveAt(i);
            }
        }
    }

    /// <summary>Removes a specific active instance, calling its <c>OnRemove</c> hook.</summary>
    public void Remove(StatusEffectInstance instance)
    {
        if (instance == null) return;
        int idx = _activeEffects.IndexOf(instance);
        if (idx < 0) return;
        _activeEffects.RemoveAt(idx);
        instance.Definition.OnRemove(instance);
    }

    /// <summary>
    /// Removes all active effects, calling <c>OnRemove</c> for each, and clears
    /// any forced attack target. Call at the end of every combat session.
    /// </summary>
    public void Clear()
    {
        for (int i = _activeEffects.Count - 1; i >= 0; i--)
        {
            _activeEffects[i].Definition.OnRemove(_activeEffects[i]);
        }
        _activeEffects.Clear();
        ForcedAttackTarget = null;
    }

    /// <summary>Returns <c>true</c> if at least one effect of type <typeparamref name="T"/> is active.</summary>
    public bool HasEffect<T>() where T : StatusEffect
    {
        for (int i = 0; i < _activeEffects.Count; i++)
        {
            if (_activeEffects[i].Definition is T)
                return true;
        }
        return false;
    }
}
