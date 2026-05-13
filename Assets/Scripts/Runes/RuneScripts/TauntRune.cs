using UnityEngine;

[CreateAssetMenu(fileName = "TauntRune", menuName = "Runes/TauntRune")]
public class TauntRune : UtilityRune
{
    [SerializeField] private TauntStatusEffect tauntEffect;
    [SerializeField] private float tauntDuration = 5f;

    [Tooltip("Chance (0–1) that the taunt is applied when triggered.")]
    [SerializeField] private float tauntChance = 0.5f;

    /// <summary>
    /// Applies the taunt to <paramref name="target"/>, forcing it to attack <paramref name="inflictor"/>.
    /// Call this from whatever trigger applies the taunt (e.g. an on-hit hook, a skill, etc.).
    /// Returns true if the taunt was successfully applied (not resisted).
    /// </summary>
    public bool TryApplyTaunt(IStatusEffectTarget target, IStatusEffectTarget inflictor)
    {
        if (tauntEffect == null || target?.StatusEffects == null) return false;
        if (Random.value > tauntChance) return false;

        return target.StatusEffects.TryApply(tauntEffect, tauntDuration, inflictor);
    }
}
