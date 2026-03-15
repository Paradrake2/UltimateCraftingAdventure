using System;
using UnityEngine;

[Serializable]
public struct CostLine
{
    [SerializeField] private Currency currency;
    [SerializeField] private long amount;

    public Currency Currency => currency;
    public long Amount => amount;
}
