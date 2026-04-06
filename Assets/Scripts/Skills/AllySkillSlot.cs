using UnityEngine;

/// <summary>
/// Holds an <see cref="AllySkill"/> reference and tracks its per-combat cooldown.
/// Assign skills in the Inspector on an ally's <c>AllyCombat</c> component.
/// </summary>
[System.Serializable]
public class AllySkillSlot
{
    [SerializeField] private AllySkill skill;

    private float remainingCooldown;

    public AllySkillSlot() { }

    public AllySkillSlot(AllySkill skill)
    {
        this.skill = skill;
    }

    public AllySkill Skill => skill;
    public float RemainingCooldown => remainingCooldown;
    public bool IsReady => skill != null && remainingCooldown <= 0f;

    /// <summary>Resets the cooldown. Pass the caster's <see cref="StatCollection"/> so
    /// attack-speed-scaling skills get the correct effective cooldown.</summary>
    public void ResetCooldown(StatCollection stats = null)
    {
        remainingCooldown = skill != null ? skill.GetEffectiveCooldown(stats) : 0f;
    }

    /// <summary>
    /// Decrements the cooldown by <paramref name="deltaTime"/>.
    /// Returns <c>true</c> if the skill is now ready to fire.
    /// </summary>
    public bool Tick(float deltaTime)
    {
        if (skill == null) return false;
        if (remainingCooldown > 0f)
            remainingCooldown = Mathf.Max(remainingCooldown - deltaTime, 0f);
        return remainingCooldown <= 0f;
    }
}
