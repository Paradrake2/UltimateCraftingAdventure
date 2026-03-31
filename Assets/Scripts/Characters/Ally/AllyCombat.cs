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

    [SerializeField] private StatCollection statCollection = new StatCollection();
    [SerializeField] private double currentHealth;
    [SerializeField] private float baseAttackSpeed = 1f;

    public StatCollection StatCollection => statCollection;
    public float BaseAttackSpeed => baseAttackSpeed;
    public double CurrentHealth
    {
        get => currentHealth;
        set => currentHealth = value;
    }
    public bool IsAlive => CurrentHealth > 0d;

    public void Initialize(StatCollection newStats)
    {
        statCollection = newStats ?? new StatCollection();
    }

    public void ResetForCombat()
    {
        CurrentHealth = GetMaxHealth();
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

    public Enemy GetAttackTarget(IReadOnlyList<Enemy> enemies)
    {
        if (!IsAlive || enemies == null || enemies.Count == 0)
        {
            return null;
        }

        return GetRandomEnemy(enemies);
    }

    public void TakeDamage(double damage)
    {
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

    public void Attack(Enemy enemy)
    {
        if (enemy == null || enemy.CombatStats == null || !enemy.CombatStats.IsAlive)
        {
            return;
        }

        enemy.CombatStats.TakeDamage(GetDamage());
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
