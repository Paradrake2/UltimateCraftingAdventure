using UnityEngine;
using System;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "StatDatabase", menuName = "Scriptable Objects/StatDatabase")]

public class StatDatabase : ScriptableObject
{
    [SerializeField] private List<Stat> stats = new List<Stat>();
    private Dictionary<string, Stat> statDictionary;
    private void OnEnable()
    {
        statDictionary = new Dictionary<string, Stat>();
        foreach (var stat in stats)
        {
            statDictionary[stat.StatName] = stat;
        }
    }
    public Stat GetStat(string statName)
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
    public Stat GetRandomStat()
    {
        if (stats.Count == 0)
        {
            Debug.LogWarning("No stats available in the database.");
            return null;
        }
        int randomIndex = UnityEngine.Random.Range(0, stats.Count);
        return stats[randomIndex];
    }
    public IReadOnlyList<Stat> Stats => stats.AsReadOnly();
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
