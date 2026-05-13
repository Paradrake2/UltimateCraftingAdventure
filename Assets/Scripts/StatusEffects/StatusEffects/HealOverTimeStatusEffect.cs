/// <summary>
/// Restores health to the affected entity at a fixed rate over the duration.
/// Requires the entity to implement <see cref="IHealable"/>.
/// </summary>
[UnityEngine.CreateAssetMenu(fileName = "HealOverTimeStatusEffect", menuName = "StatusEffects/HealOverTime")]
public class HealOverTimeStatusEffect : StatusEffect
{
    [UnityEngine.Tooltip("Health restored per second.")]
    [UnityEngine.SerializeField] private float healPerSecond = 5f;

    public override void OnTick(StatusEffectInstance instance, float deltaTime)
    {
        if (instance.Manager.Owner is IHealable healable)
            healable.Heal(healPerSecond * deltaTime);
    }
}
