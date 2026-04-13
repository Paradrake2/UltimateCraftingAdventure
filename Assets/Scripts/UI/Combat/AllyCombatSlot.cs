using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AllyCombatSlot : MonoBehaviour
{
    [SerializeField] private Ally ally;
    [SerializeField] private Image allyIcon;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject attackBar;
    [SerializeField] private GameObject barrierBar;
    public Ally Ally => ally;
    public void Initialize(Ally newAlly)
    {
        ally = newAlly;
        if (ally != null)
        {
            allyIcon.sprite = ally.Icon;
            StartCoroutine(HealthBarCoroutine());
            StartCoroutine(AttackBarCoroutine());
            StartCoroutine(BarrierBarCoroutine());
        }
    }
    private IEnumerator HealthBarCoroutine()
    {
        while (ally != null && ally.CombatStats != null)
        {
            double currentHealth = ally.CombatStats.CurrentHealth;
            double maxHealth = ally.CombatStats.GetMaxHealth();
            float healthPercent = maxHealth > 0 ? (float)(currentHealth / maxHealth) : 0f;

            if (healthBar != null)
            {
                Image healthFill = healthBar.GetComponent<Image>();
                if (healthFill != null)
                {
                    healthFill.fillAmount = healthPercent;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
    private IEnumerator AttackBarCoroutine()
    {
        while (ally != null && ally.CombatStats != null)
        {
            float attackInterval = ally.CombatStats.GetAttackInterval();
            float elapsedTime = 0f;

            while (elapsedTime < attackInterval)
            {
                elapsedTime += Time.deltaTime;
                float attackPercent = Mathf.Clamp01(elapsedTime / attackInterval);

                if (attackBar != null)
                {
                    Image attackFill = attackBar.GetComponent<Image>();
                    if (attackFill != null)
                    {
                        attackFill.fillAmount = attackPercent;
                    }
                }

                yield return null;
            }
        }
    }
    private IEnumerator BarrierBarCoroutine()
    {
        while (ally != null && ally.CombatStats != null)
        {
            double currentBarrier = ally.CombatStats.CurrentBarrier;
            double maxBarrier = ally.CombatStats.GetMaxBarrier();
            float barrierPercent = maxBarrier > 0 ? (float)(currentBarrier / maxBarrier) : 0f;

            if (barrierBar != null)
            {
                Image barrierFill = barrierBar.GetComponent<Image>();
                if (barrierFill != null)
                {
                    barrierFill.fillAmount = barrierPercent;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}
