using UnityEngine;

/// <summary>
/// Holds an <see cref="AllySkill"/> reference and tracks its per-combat cooldown.
/// Each slot has a fixed <see cref="SkillSlotType"/> that enforces which skills may be placed in it.
/// </summary>
[System.Serializable]
public class AllySkillSlot
{
    [SerializeField] private AllySkill skill;
    [SerializeField] private SkillSlotType slotType;

    private float remainingCooldown;

    public AllySkillSlot() { }

    /// <summary>Creates an empty typed slot.</summary>
    public AllySkillSlot(SkillSlotType slotType)
    {
        this.slotType = slotType;
    }

    /// <summary>Creates a typed slot and immediately assigns <paramref name="skill"/>.
    /// The slot type is derived from the skill.</summary>
    public AllySkillSlot(AllySkill skill)
    {
        this.skill = skill;
        this.slotType = skill != null ? skill.SlotType : default;
    }

    public AllySkill Skill => skill;
    public SkillSlotType SlotType => slotType;
    public float RemainingCooldown => remainingCooldown;
    public bool IsReady => skill != null && remainingCooldown <= 0f;

    /// <summary>Assigns a skill to this slot without changing the slot type.
    /// Pass <c>null</c> to clear the slot.</summary>
    internal void SetSkill(AllySkill newSkill)
    {
        skill = newSkill;
        remainingCooldown = 0f;
    }

    /// <summary>Returns <c>true</c> if <paramref name="candidate"/> is compatible with this slot's type.</summary>
    public bool CanAccept(AllySkill candidate, out string failureReason)
    {
        failureReason = null;
        if (candidate == null) return true;
        if (candidate.SlotType != slotType)
        {
            failureReason = $"Skill '{candidate.name}' requires a {candidate.SlotType} slot but this is a {slotType} slot.";
            return false;
        }
        return true;
    }

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
