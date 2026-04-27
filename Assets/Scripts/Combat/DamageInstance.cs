public readonly struct DamageInstance
{
    public readonly DamageType Type;
    public readonly DamageAttribute? Attribute; // null = pure delivery-type damage, non-null = elemental bonus
    public readonly double Amount;

    /// Delivery-type damage (no elemental component).
    public DamageInstance(DamageType type, double amount)
    {
        Type = type;
        Attribute = null;
        Amount = amount;
    }

    /// Elemental attribute bonus damage.
    public DamageInstance(DamageAttribute attribute, double amount)
    {
        Type = DamageType.Physical; // unused when Attribute is set; kept for struct completeness
        Attribute = attribute;
        Amount = amount;
    }

    /// <summary>
    /// Returns a copy of this instance with <paramref name="newAmount"/> as
    /// the damage value, preserving the original type and attribute.
    /// </summary>
    public DamageInstance WithAmount(double newAmount) =>
        Attribute.HasValue
            ? new DamageInstance(Attribute.Value, newAmount)
            : new DamageInstance(Type, newAmount);
}
