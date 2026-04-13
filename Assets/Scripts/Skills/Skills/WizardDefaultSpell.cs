using UnityEngine;

[CreateAssetMenu(fileName = "WizardDefaultSpell", menuName = "Skills/Default/Wizard Default Spell")]
public class WizardDefaultSpell : AllySkill
{
    public override void Execute(SkillContext context)
    {
        Enemy target = context.Caster.CombatStats.GetAttackTarget(context.Enemies);
        if (target == null) return;

        var instances = BuildDamageInstances(context.Caster);
        if (instances.Count > 0)
            target.CombatStats.TakeDamage(instances);
    }
}
