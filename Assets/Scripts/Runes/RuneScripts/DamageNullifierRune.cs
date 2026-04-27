using UnityEngine;

[CreateAssetMenu(fileName = "DamageNullifierRune", menuName = "Runes/DamageNullifierRune")]
public class DamageNullifierRune : DefenseRune
{
    [SerializeField] private float nullificationChance = 0.5f;
    /// <summary>1 = 100% nullification (all damage removed), 0.5 = 50% reduction.</summary>
    [SerializeField] [Range(0f, 1f)] private float nullificationAmount = 1f;

    public override void ApplyEffect(DefenseRuneContext ctx)
    {
        if (Random.value > nullificationChance) return;

        for (int i = ctx.DamageInstances.Count - 1; i >= 0; i--)
        {
            DamageInstance d = ctx.DamageInstances[i];
            double reduced = d.Amount * (1.0 - nullificationAmount);
            if (reduced <= 0.0)
                ctx.DamageInstances.RemoveAt(i);
            else
                ctx.DamageInstances[i] = d.WithAmount(reduced);
        }
    }
}
