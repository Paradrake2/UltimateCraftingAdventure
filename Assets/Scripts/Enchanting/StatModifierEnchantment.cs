using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatModifierEnchantment", menuName = "Enchanting/EnchantmentEffects/StatModifierEnchantment")]
public class StatModifierEnchantment : ScriptableObject, IEnchantmentEffect
{
    [SerializeField] private string enchantmentName;
    [SerializeField] private Sprite icon;
    [SerializeField] private List<Stat> stats = new List<Stat>();
    [SerializeField] private float modifierAmount;
    public void ApplyEffect(Equipment equipment)
    {
        if (equipment == null || stats == null || stats.Count == 0)
        {
            return;
        }

        foreach (var stat in equipment.Stats.Stats)
        {
            if (stats.Contains(stat.Stat))
            {
                equipment.ApplyStatModifier(stat.Stat, modifierAmount);
            }
        }
    }
    public void AddStat(Stat stat)
    {
        if (stat == null)
        {
            return;
        }

        if (stats == null)
        {
            stats = new List<Stat>();
        }

        if (!stats.Contains(stat))
        {
            stats.Add(stat);
        }
    }
    public void SetModifierAmount(float amount)
    {
        modifierAmount = amount;
    }
    public void SetStats(List<Stat> newStats)
    {
        stats = newStats ?? new List<Stat>();
    }
}
