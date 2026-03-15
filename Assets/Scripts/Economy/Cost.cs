using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Cost
{
    [SerializeField] private List<CostLine> lines = new List<CostLine>();
    public IReadOnlyList<CostLine> Lines => lines;
}
