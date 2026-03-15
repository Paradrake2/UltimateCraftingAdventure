using UnityEngine;

public enum StatType
{
    Stat,
    Attribute,
    Resistance,
    Buff,
    Debuff
}


[CreateAssetMenu(fileName = "Stat", menuName = "Scriptable Objects/Stat")]
public class Stat : ScriptableObject
{
    [SerializeField] private string statName;
    [SerializeField] private StatType statType;
    public string StatName => statName;
}
