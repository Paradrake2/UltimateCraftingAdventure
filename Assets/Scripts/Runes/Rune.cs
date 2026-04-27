using UnityEngine;

public enum RuneType
{
    Defense,
    Utility,
}

/// <summary>
/// Base class for all runes. Extend <see cref="DefenseRune"/> or
/// <see cref="UtilityRune"/> rather than this class directly.
/// </summary>
public abstract class Rune : ScriptableObject
{
    [SerializeField] private string runeName;
    [SerializeField] private Sprite icon;

    public string RuneName => runeName;
    public Sprite Icon => icon;

    /// <summary>Determined by the subclass; not a serialized field.</summary>
    public abstract RuneType RuneType { get; }
}
