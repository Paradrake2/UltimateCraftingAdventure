using UnityEngine;

public enum StatType
{
    Damaging,
    Defensive,
    Resistance
}


[CreateAssetMenu(fileName = "Stat", menuName = "Scriptable Objects/Stat")]
public class Stat : ScriptableObject
{
    [SerializeField] private string statName;
    [SerializeField] private StatType statType;
    [SerializeField] private float baseValue;
    [SerializeField] private Sprite icon;
    [Tooltip("Lower numbers appear higher in stat lists. Stats with the same order value are sorted alphabetically.")]
    [SerializeField] private int displayOrder = 0;
    public string StatName => statName;
    public StatType StatType => statType;
    public float BaseValue => baseValue;
    public Sprite Icon => icon;
    public int DisplayOrder => displayOrder;
}
