using System.Collections.Generic;

/// <summary>
/// Passed to <see cref="AllySkill.Execute"/> with everything a skill needs to act on the battle.
/// <para><see cref="Allies"/> contains only living allies (including the caster).</para>
/// <para><see cref="Enemies"/> contains active enemies — always check
/// <c>enemy.CombatStats.IsAlive</c> before applying effects.</para>
/// </summary>
public readonly struct SkillContext
{
    /// <summary>The ally that is using the skill.</summary>
    public readonly Ally Caster;

    /// <summary>All currently living allies, including the caster.</summary>
    public readonly IReadOnlyList<Ally> Allies;

    /// <summary>All active enemies. Check <c>CombatStats.IsAlive</c> before targeting.</summary>
    public readonly IReadOnlyList<Enemy> Enemies;

    public SkillContext(Ally caster, IReadOnlyList<Ally> allies, IReadOnlyList<Enemy> enemies)
    {
        Caster = caster;
        Allies = allies;
        Enemies = enemies;
    }
}
