using UnityEngine;
using System;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "StatDatabase", menuName = "Scriptable Objects/StatDatabase")]

public class StatDatabase : ScriptableObject
{
    [SerializeField] private List<StatValue> stats = new List<StatValue>();
    private Dictionary<string, StatValue> statDictionary;
    private void OnEnable()
    {
        statDictionary = new Dictionary<string, StatValue>();
        foreach (var statValue in stats)
        {
            statDictionary[statValue.Stat.StatName] = statValue;
        }
    }
    public StatValue GetStatValue(string statName)
    {
        if (statDictionary.TryGetValue(statName, out var statValue))
        {
            return statValue;
        }
        else
        {
            Debug.LogWarning($"Stat '{statName}' not found in database.");
            return null;
        }
    }
    public IReadOnlyList<StatValue> Stats => stats.AsReadOnly();
    private static StatDatabase instance;
    public static StatDatabase Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<StatDatabase>("StatDatabase");
                if (instance == null)
                {
                    Debug.LogError("StatDatabase asset not found in Resources folder.");
                }
            }
            return instance;
        }
    }
    
}
