using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyCombat
{
    private const string AttackSpeedStatName = "AttackSpeed";
    private const string DamageStatName = "Damage";
    private const string HealthStatName = "Health";
    private const float MinimumAttackInterval = 0.01f;
    private const float MinimumHealth = 1f;
    private const string DamageSuffix = "Damage";
    private const string ResistanceSuffix = "Resistance";
    private const string DamageMultiplierSuffix = "DamageMultiplier";

    [SerializeField] private StatCollection statCollection = new StatCollection();
    [SerializeField] private double currentHealth;
    [SerializeField] private double currentBarrier = 0d;
    [SerializeField] private float baseAttackSpeed = 1f;
    [SerializeField] private AttackTargeting attackTargeting = AttackTargeting.Single;
    [SerializeField] private int maxTargets = 2;
    [SerializeField] private Enemy enemy;

    public StatCollection StatCollection => statCollection;
    public float BaseAttackSpeed => baseAttackSpeed;
    public AttackTargeting AttackTargeting => attackTargeting;
    public int MaxTargets => maxTargets;
    public double CurrentHealth
    {
        get => currentHealth;
        set => currentHealth = value;
    }
    public double CurrentBarrier
    {
        get => currentBarrier;
        set => currentBarrier = value;
    }
    public bool IsAlive => CurrentHealth > 0d;

    public void Initialize(StatCollection newStats, Enemy parentEnemy)
    {
        statCollection = newStats ?? new StatCollection();
        enemy = parentEnemy;
    }

    public void ResetForCombat()
    {
        CurrentHealth = GetMaxHealth();
        CurrentBarrier = GetMaxBarrier();
    }

    public float GetAttackInterval()
    {
        float normalizedBaseAttackSpeed = Mathf.Max(baseAttackSpeed, MinimumAttackInterval);
        float attackSpeed = statCollection != null ? statCollection.GetStatValue(AttackSpeedStatName) : 0f;

        if (attackSpeed <= 0f)
        {
            return normalizedBaseAttackSpeed;
        }

        return Mathf.Max(normalizedBaseAttackSpeed / attackSpeed, MinimumAttackInterval);
    }

    public double GetDamage()
    {
        if (statCollection == null)
        {
            return 0d;
        }

        return statCollection.GetStatValue(DamageStatName);
    }

    public double GetMaxHealth()
    {
        if (statCollection == null)
        {
            return MinimumHealth;
        }

        return Mathf.Max(statCollection.GetStatValue(HealthStatName), MinimumHealth);
    }

    public double GetMaxBarrier()
    {
        return statCollection != null ? Mathf.Max(statCollection.GetStatValue("Barrier"), 0f) : 0d;
    }

    public Ally GetAttackTarget(IReadOnlyList<Ally> allies)
    {
        if (!IsAlive || allies == null || allies.Count == 0)
        {
            return null;
        }

        return GetRandomLivingAlly(allies);
    }

    public void TakeDamage(IReadOnlyList<DamageInstance> instances)
    {
        if (instances == null || instances.Count == 0) return;

        double totalDamage = 0d;
        for (int i = 0; i < instances.Count; i++)
        {
            DamageInstance hit = instances[i];
            string resistStat = hit.Type.ToString() + ResistanceSuffix;
            float resistance = statCollection != null
                ? Mathf.Clamp(statCollection.GetStatValue(resistStat), 0f, 100f)
                : 0f;
            totalDamage += hit.Amount * (1f - resistance / 100f);
        }

        ApplyDamageToHealthPool(totalDamage);
    }

    private void ApplyDamageToHealthPool(double damage)
    {
        if (CurrentBarrier > 0)
        {
            double remainingDamage = damage - CurrentBarrier;
            CurrentBarrier = Mathf.Max((float)(CurrentBarrier - damage), 0);
            damage = remainingDamage;
        }

        if (damage <= 0) return;

        CurrentHealth -= damage;
        if (CurrentHealth <= 0d)
        {
            Die();
        }
    }

    public void Die()
    {
        CurrentHealth = 0d;
        CurrentBarrier = 0d;
    }

    public List<DamageInstance> BuildAttackInstances()
    {
        var instances = new List<DamageInstance>();
        if (statCollection == null) return instances;

        foreach (DamageType type in System.Enum.GetValues(typeof(DamageType)))
        {
            string damageStat = type.ToString() + DamageSuffix;
            float baseDamage = statCollection.GetStatValue(damageStat);
            if (baseDamage <= 0f) continue;

            string multiplierStat = type.ToString() + DamageMultiplierSuffix;
            float multiplier = 1f + statCollection.GetStatValue(multiplierStat) / 100f;
            instances.Add(new DamageInstance(type, baseDamage * multiplier));
        }

        if (instances.Count == 0)
        {
            float legacyDamage = statCollection.GetStatValue(DamageStatName);
            if (legacyDamage > 0f)
                instances.Add(new DamageInstance(DamageType.Physical, legacyDamage));
        }

        return instances;
    }

    public void Attack(Ally ally)
    {
        if (ally == null || ally.CombatStats == null || !ally.CombatStats.IsAlive)
        {
            return;
        }

        ally.CombatStats.TakeDamage(BuildAttackInstances());
    }

    public bool AttackTargets(IReadOnlyList<Ally> allies)
    {
        if (!IsAlive || allies == null || allies.Count == 0) return false;

        var instances = BuildAttackInstances();

        if (attackTargeting == AttackTargeting.All)
        {
            bool attacked = false;
            for (int i = 0; i < allies.Count; i++)
            {
                Ally a = allies[i];
                if (a != null && a.CombatStats != null && a.CombatStats.IsAlive)
                {
                    a.CombatStats.TakeDamage(instances);
                    attacked = true;
                }
            }
            return attacked;
        }

        if (attackTargeting == AttackTargeting.Multi)
        {
            return AttackMultipleTargets(allies, instances);
        }

        Ally target = GetAttackTarget(allies);
        if (target == null) return false;
        Attack(target);
        return true;
    }

    private bool AttackMultipleTargets(IReadOnlyList<Ally> allies, List<DamageInstance> instances)
    {
        // Collect living targets into a temporary list
        var pool = new List<Ally>(allies.Count);
        for (int i = 0; i < allies.Count; i++)
        {
            Ally a = allies[i];
            if (a != null && a.CombatStats != null && a.CombatStats.IsAlive)
                pool.Add(a);
        }

        int count = Mathf.Min(maxTargets, pool.Count);
        if (count <= 0) return false;

        for (int i = 0; i < count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, pool.Count);
            // Swap so we don't pick the same target twice
            Ally chosen = pool[randomIndex];
            pool[randomIndex] = pool[i];
            pool[i] = chosen;
            chosen.CombatStats.TakeDamage(instances);
        }
        return true;
    }

    private Ally GetRandomLivingAlly(IReadOnlyList<Ally> allies)
    {
        int validAllyCount = 0;
        for (int i = 0; i < allies.Count; i++)
        {
            if (allies[i] != null && allies[i].CombatStats != null && allies[i].CombatStats.IsAlive)
            {
                validAllyCount++;
            }
        }

        if (validAllyCount == 0)
        {
            return null;
        }

        int selectedIndex = Random.Range(0, validAllyCount);
        for (int i = 0; i < allies.Count; i++)
        {
            if (allies[i] == null || allies[i].CombatStats == null || !allies[i].CombatStats.IsAlive)
            {
                continue;
            }

            if (selectedIndex == 0)
            {
                return allies[i];
            }

            selectedIndex--;
        }

        return null;
    }
}