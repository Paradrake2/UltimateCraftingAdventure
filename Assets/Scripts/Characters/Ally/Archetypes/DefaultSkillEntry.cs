using UnityEngine;

/// <summary>
/// Pairs a <see cref="SkillSlotType"/> with an optional default <see cref="AllySkill"/>
/// for use in <see cref="AllyArchetype"/> slot configuration.
/// </summary>
[System.Serializable]
public class DefaultSkillEntry
{
    [Tooltip("The type of skill slot this entry represents.")]
    [SerializeField] private SkillSlotType slotType;

    [Tooltip("The default skill pre-assigned to this slot. Leave empty for no default.")]
    [SerializeField] private AllySkill defaultSkill;

    public SkillSlotType SlotType => slotType;
    public AllySkill DefaultSkill => defaultSkill;
}
