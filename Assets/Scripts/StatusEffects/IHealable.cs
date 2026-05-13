/// <summary>
/// Implemented by entities whose health pool can be restored (used by HealOverTime and similar effects).
/// </summary>
public interface IHealable
{
    double CurrentHealth { get; }
    double MaxHealth { get; }

    /// <summary>Adds <paramref name="amount"/> to current health, clamped to <see cref="MaxHealth"/>.</summary>
    void Heal(double amount);
}
