using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class EnemyCombatSlot : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    [SerializeField] private Image icon;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject attackBar;
    [SerializeField] private GameObject barrierBar;

    public void Initialize(Enemy newEnemy)
    {
        enemy = newEnemy;
        if (enemy != null)
        {
            enemy.CombatStats.Initialize(enemy.Stats);
            icon.sprite = enemy.Icon;
            StartCoroutine(HealthBarCoroutine());
            StartCoroutine(AttackBarCoroutine());
            StartCoroutine(BarrierBarCoroutine());
        }
    }

    private IEnumerator HealthBarCoroutine()
    {
        while (enemy != null && enemy.CombatStats != null)
        {
            if (!enemy.CombatStats.IsAlive)
            {
                Destroy(gameObject);
                yield break;
            }

            double currentHealth = enemy.CombatStats.CurrentHealth;
            double maxHealth = enemy.CombatStats.GetMaxHealth();
            float healthPercent = maxHealth > 0 ? (float)(currentHealth / maxHealth) : 0f;

            if (healthBar != null)
            {
                Image healthFill = healthBar.GetComponent<Image>();
                if (healthFill != null)
                    healthFill.fillAmount = healthPercent;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator AttackBarCoroutine()
    {
        while (enemy != null && enemy.CombatStats != null)
        {
            float attackInterval = enemy.CombatStats.GetAttackInterval();
            float elapsedTime = 0f;

            while (elapsedTime < attackInterval)
            {
                elapsedTime += Time.deltaTime;
                float attackPercent = Mathf.Clamp01(elapsedTime / attackInterval);

                if (attackBar != null)
                {
                    Image attackFill = attackBar.GetComponent<Image>();
                    if (attackFill != null)
                        attackFill.fillAmount = attackPercent;
                }

                yield return null;
            }
        }
    }

    private IEnumerator BarrierBarCoroutine()
    {
        while (enemy != null && enemy.CombatStats != null)
        {
            double currentBarrier = enemy.CombatStats.CurrentBarrier;
            double maxBarrier = enemy.CombatStats.GetMaxBarrier();
            float barrierPercent = maxBarrier > 0 ? (float)(currentBarrier / maxBarrier) : 0f;

            if (barrierBar != null)
            {
                Image barrierFill = barrierBar.GetComponent<Image>();
                if (barrierFill != null)
                    barrierFill.fillAmount = barrierPercent;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}
