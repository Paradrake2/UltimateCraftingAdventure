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
    public string StatName => statName;
    public StatType StatType => statType;
    public float BaseValue => baseValue;
}
