using UnityEngine;

[CreateAssetMenu(fileName = "WarriorDefaultSlash", menuName = "Skills/Default/Warrior Default Slash")]
public class WarriorDefaultSlash : AllySkill
{
    public override void Execute(SkillContext context)
    {
        Enemy target = context.Caster.CombatStats.GetAttackTarget(context.Enemies);
        if (target == null) return;

        var instances = BuildDamageInstances(context.Caster.CombatStats.StatCollection);
        if (instances.Count > 0)
            target.CombatStats.TakeDamage(instances);
    }
}
