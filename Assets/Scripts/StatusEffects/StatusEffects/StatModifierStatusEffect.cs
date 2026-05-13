/// <summary>
/// Adds or removes a flat value from a stat for the duration of the effect.
/// Use a positive <see cref="amount"/> for a buff and a negative value for a debuff.
///
/// <para>Example uses: damage boost, attack speed increase, armor reduction, etc.</para>
/// </summary>
[UnityEngine.CreateAssetMenu(fileName = "StatModifierStatusEffect", menuName = "StatusEffects/StatModifier")]
public class StatModifierStatusEffect : StatusEffect
{
    [UnityEngine.SerializeField] private Stat stat;
    [UnityEngine.SerializeField] private float amount;

    public override void OnApply(StatusEffectInstance instance)
    {
        if (stat != null)
            instance.Manager.Stats?.AddStatValue(stat, amount);
    }

    public override void OnRemove(StatusEffectInstance instance)
    {
        if (stat != null)
            instance.Manager.Stats?.AddStatValue(stat, -amount);
    }
}
