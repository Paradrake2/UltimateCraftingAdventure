/// <summary>
/// Forces the afflicted entity to direct all attacks at the inflictor for the duration.
/// The inflictor is stored on the <see cref="StatusEffectManager.ForcedAttackTarget"/> property;
/// the entity's combat targeting logic is responsible for honouring it.
/// </summary>
[UnityEngine.CreateAssetMenu(fileName = "TauntStatusEffect", menuName = "StatusEffects/Taunt")]
public class TauntStatusEffect : StatusEffect
{
    public override void OnApply(StatusEffectInstance instance)
    {
        instance.Manager.ForcedAttackTarget = instance.Inflictor;
    }

    public override void OnRemove(StatusEffectInstance instance)
    {
        // Only clear the forced target if it still points to this taunt's inflictor,
        // so a newer taunt applied later is not accidentally cleared.
        if (instance.Manager.ForcedAttackTarget == instance.Inflictor)
            instance.Manager.ForcedAttackTarget = null;
    }
}
