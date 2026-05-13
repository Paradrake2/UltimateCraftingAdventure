using UnityEngine;

[CreateAssetMenu(fileName = "TauntSkill", menuName = "Skills/TauntSkill")]
public class AllyAOETauntSkill : AllySkill
{
    [SerializeField] private TauntStatusEffect tauntEffect;
    [SerializeField] private float tauntDuration = 5f;

    public override void Execute(SkillContext context)
    {
        Debug.Log($"Executing {name} for {context.Caster.name}, taunting {context.Enemies.Count} enemies.");
        foreach (var enemy in context.Enemies)
        {
            if (enemy == null || !enemy.CombatStats.IsAlive) continue;
            enemy.StatusEffects.TryApply(tauntEffect, tauntDuration, context.Caster);
        }
    }
}
