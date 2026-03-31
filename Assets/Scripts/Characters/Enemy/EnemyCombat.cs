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

    public Ally GetAttackTarget(IReadOnlyList<Ally> allies)
    {
        if (!IsAlive || allies == null || allies.Count == 0)
        {
            return null;
        }

        return GetRandomLivingAlly(allies);
    }

    public void TakeDamage(double damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth < 0d)
        {
            CurrentHealth = 0d;
        }
    }

    public void Attack(Ally ally)
    {
        if (ally == null || ally.CombatStats == null || !ally.CombatStats.IsAlive)
        {
            return;
        }

        ally.CombatStats.TakeDamage(GetDamage());
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