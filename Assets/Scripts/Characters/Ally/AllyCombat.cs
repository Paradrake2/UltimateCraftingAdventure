using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AllyCombat
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
    [SerializeField] private float baseAttackSpeed = 1f;
    [SerializeField] private double currentBarrier = 0d;

    public StatCollection StatCollection => statCollection;
    public float BaseAttackSpeed => baseAttackSpeed;
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

    public void Initialize(StatCollection newStats)
    {
        statCollection = newStats ?? new StatCollection();
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
    public double GetEffectiveHealth()
    {
        return GetMaxHealth() + CurrentBarrier;
    }
    public double GetMaxBarrier()
    {
        return Mathf.Max(statCollection.GetStatValue("Barrier"), 0f);
    }
    public double GetEffectiveHealthWithMaxBarrier()
    {
        return GetMaxHealth() + GetMaxBarrier();
    }
    public double GetHealthPercent()
    {
        double effectiveHealth = GetEffectiveHealth();
        return effectiveHealth > 0 ? CurrentHealth / effectiveHealth : 0d;
    }
    public double GetHealthPercentWithMaxBarrier()
    {
        double effectiveHealth = GetEffectiveHealthWithMaxBarrier();
        return effectiveHealth > 0 ? CurrentHealth / effectiveHealth : 0d;
    }
    public double GetBarrierPercent()
    {
        double maxBarrier = GetMaxBarrier();
        return maxBarrier > 0 ? CurrentBarrier / maxBarrier : 0d;
    }
    public Enemy GetAttackTarget(IReadOnlyList<Enemy> enemies)
    {
        if (!IsAlive || enemies == null || enemies.Count == 0)
        {
            return null;
        }

        return GetRandomEnemy(enemies);
    }

    public void TakeDamage(System.Collections.Generic.IReadOnlyList<DamageInstance> instances)
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
        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
        }
    }

    public void Die()
    {
        // placeholder
    }

    public System.Collections.Generic.List<DamageInstance> BuildAttackInstances()
    {
        var instances = new System.Collections.Generic.List<DamageInstance>();
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

    public void Attack(Enemy enemy)
    {
        if (enemy == null || enemy.CombatStats == null || !enemy.CombatStats.IsAlive)
        {
            return;
        }

        enemy.CombatStats.TakeDamage(BuildAttackInstances());
    }

    private Enemy GetRandomEnemy(IReadOnlyList<Enemy> enemies)
    {
        int validEnemyCount = 0;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] != null && enemies[i].CombatStats != null && enemies[i].CombatStats.IsAlive)
            {
                validEnemyCount++;
            }
        }

        if (validEnemyCount == 0)
        {
            return null;
        }

        int selectedIndex = Random.Range(0, validEnemyCount);
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] == null || enemies[i].CombatStats == null || !enemies[i].CombatStats.IsAlive)
            {
                continue;
            }

            if (selectedIndex == 0)
            {
                return enemies[i];
            }

            selectedIndex--;
        }

        return null;
    }

    private void ApplyAugmentEffects(Equipment equipment)
    {
        foreach (var augmentHolder in equipment.Augments)
        {
            if (augmentHolder.Augment != null && !augmentHolder.BeenUsed)
            {
                augmentHolder.Augment.Effect.ApplyEffect(equipment, null);
                // augmentHolder.MarkAsUsed(); // Mark the augment as used after applying its effect
            }
        }
    }
}
