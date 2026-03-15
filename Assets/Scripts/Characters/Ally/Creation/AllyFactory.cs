using System.Collections.Generic;
using UnityEngine;

public static class AllyFactory
{
    public static bool TryCreateExplicit(CreateAlly request, out Ally ally, out string failureReason)
    {
        ally = null;
        failureReason = null;

        if (request == null)
        {
            failureReason = "Cannot create ally: request is null.";
            return false;
        }

        if (request.Archetype == null)
        {
            failureReason = "Cannot create ally: archetype is null.";
            return false;
        }

        string allyName = string.IsNullOrWhiteSpace(request.AllyName) ? "Ally" : request.AllyName;

        StatCollection stats;
        if (request.StatsOverride != null && request.StatsOverride.Stats.Count > 0)
        {
            stats = request.StatsOverride.Clone();
        }
        else
        {
            stats = request.Archetype.BaseStats.Clone();
        }

        Sprite icon = request.IconOverride != null ? request.IconOverride : request.Archetype.Icon;

        ally = Ally.CreateRuntime(allyName, request.Archetype, icon, stats);

        if (request.StartingEquipment != null)
        {
            TryEquipAll(ally, request.StartingEquipment);
        }

        if (!ally.ValidateLoadout(out failureReason))
        {
            ally = null;
            return false;
        }

        return true;
    }

    public static bool TryCreateRandom(AllyGenerationConfig config, out Ally ally, out string failureReason)
    {
        ally = null;
        failureReason = null;

        if (config == null)
        {
            failureReason = "Cannot create ally: generation config is null.";
            return false;
        }

        if (config.PossibleArchetypes == null || config.PossibleArchetypes.Count == 0)
        {
            failureReason = "Cannot create ally: no possible archetypes configured.";
            return false;
        }

        var archetype = config.PossibleArchetypes[Random.Range(0, config.PossibleArchetypes.Count)];
        if (archetype == null)
        {
            failureReason = "Cannot create ally: selected archetype was null.";
            return false;
        }

        string allyName = RollName(config.NamePool);
        var stats = archetype.BaseStats.Clone();

        if (config.AdditiveStatRolls != null)
        {
            foreach (var roll in config.AdditiveStatRolls)
            {
                if (roll == null) continue;
                if (roll.Stat == null) continue;
                stats.AddStatValue(roll.Stat, roll.RollValue());
            }
        }

        ally = Ally.CreateRuntime(allyName, archetype, archetype.Icon, stats);

        if (!ally.ValidateLoadout(out failureReason))
        {
            ally = null;
            return false;
        }

        return true;
    }

    private static string RollName(IReadOnlyList<string> pool)
    {
        if (pool == null || pool.Count == 0) return "Ally";

        // Avoid empty/whitespace names if the pool has bad entries.
        for (int i = 0; i < pool.Count; i++)
        {
            string candidate = pool[Random.Range(0, pool.Count)];
            if (!string.IsNullOrWhiteSpace(candidate)) return candidate;
        }

        return "Ally";
    }

    private static void TryEquipAll(Ally ally, IReadOnlyList<Equipment> equipment)
    {
        if (ally == null || equipment == null) return;

        for (int i = 0; i < equipment.Count; i++)
        {
            var item = equipment[i];
            if (item == null) continue;
            ally.TryEquip(item, out _);
        }
    }
}
