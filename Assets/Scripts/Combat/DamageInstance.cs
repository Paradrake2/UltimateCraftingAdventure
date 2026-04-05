public readonly struct DamageInstance
{
    public readonly DamageType Type;
    public readonly double Amount;

    public DamageInstance(DamageType type, double amount)
    {
        Type = type;
        Amount = amount;
    }
}
