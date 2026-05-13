/// <summary>
/// Reduces a configurable stat (e.g. PhysicalResistance) by a flat amount for the duration.
/// On removal the stat is restored by the same amount.
/// </summary>
[UnityEngine.CreateAssetMenu(fileName = "DefenseDownStatusEffect", menuName = "StatusEffects/DefenseDown")]
public class DefenseDownStatusEffect : StatusEffect
{
    [UnityEngine.SerializeField] private Stat stat;
    [UnityEngine.SerializeField] private float reductionAmount = 10f;

    public override void OnApply(StatusEffectInstance instance)
    {
        if (stat != null)
            instance.Manager.Stats?.AddStatValue(stat, -reductionAmount);
    }

    public override void OnRemove(StatusEffectInstance instance)
    {
        if (stat != null)
            instance.Manager.Stats?.AddStatValue(stat, reductionAmount);
    }
}
