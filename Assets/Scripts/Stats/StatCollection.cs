using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatCollection
{
    [SerializeField] private List<StatValue> stats = new List<StatValue>();
    public IReadOnlyList<StatValue> Stats => stats;

    public StatCollection Clone()
    {
        var copy = new StatCollection();
        if (stats == null) return copy;

        foreach (var statValue in stats)
        {
            if (statValue == null) continue;
            if (statValue.Stat == null) continue;
            copy.SetStat(statValue.Stat, statValue.Value);
        }

        return copy;
    }

    public void SetStat(Stat stat, float value)
    {
        StatValue existingStat = stats.Find(s => s.Stat == stat);
        if (existingStat != null)
        {
            existingStat.SetValue(value);
        }
        else
        {
            stats.Add(new StatValue(stat, value));
        }
    }
    public float GetStatValue(Stat stat)
    {
        StatValue existingStat = stats.Find(s => s.Stat == stat);
        return existingStat != null ? existingStat.Value : 0f;
    }
    public void AddStatValue(Stat stat, float amount)
    {
        StatValue existingStat = stats.Find(s => s.Stat == stat);
        if (existingStat != null)
        {
            existingStat.AddValue(amount);
        }
        else
        {
            stats.Add(new StatValue(stat, amount));
        }
    }
    public bool TryGetStatValue(Stat stat, out float value)
    {
        StatValue existingStat = stats.Find(s => s.Stat == stat);
        if (existingStat != null)
        {
            value = existingStat.Value;
            return true;
        }
        value = 0f;
        return false;
    }
    public bool TryGetStat(string statName, out float value)
    {
        StatValue existingStat = stats.Find(s => s.Stat.StatName == statName);
        if (existingStat != null)
        {
            value = existingStat.Value;
            return true;
        }
        value = 0f;
        return false;
    }
    public float GetStatValue(string statName)
    {
        StatValue existingStat = stats.Find(s => s.Stat.StatName == statName);
        return existingStat != null ? existingStat.Value : 0f;
    }
    public StatValue GetRandomStat()
    {
        if (stats == null || stats.Count == 0) return null;
        int index = Random.Range(0, stats.Count);
        return stats[index];
    }
    public void AddStat(StatValue statValue)
    {
        if (statValue == null || statValue.Stat == null) return;
        SetStat(statValue.Stat, statValue.Value);
    }
}
