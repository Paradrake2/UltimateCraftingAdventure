using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Enemy")]
public class Enemy : ScriptableObject
{
	[SerializeField] private string enemyName;
	[SerializeField] private Sprite icon;
	[SerializeField] private StatCollection stats = new StatCollection();
	[SerializeField] private EnemyCombat combatStats = new EnemyCombat();

	public string EnemyName => string.IsNullOrWhiteSpace(enemyName) ? name : enemyName;
	public Sprite Icon => icon;
	public StatCollection Stats => stats;
	public EnemyCombat CombatStats => combatStats;

	public void Initialize(string newEnemyName, Sprite newIcon, StatCollection newStats)
	{
		enemyName = newEnemyName;
		icon = newIcon;
		stats = newStats ?? new StatCollection();

		combatStats ??= new EnemyCombat();
		combatStats.Initialize(stats);
	}

	public static Enemy CreateRuntime(Enemy template)
	{
		if (template == null)
		{
			return null;
		}

		Enemy enemy = CreateInstance<Enemy>();
		StatCollection runtimeStats = template.stats != null ? template.stats.Clone() : new StatCollection();
		enemy.Initialize(template.EnemyName, template.icon, runtimeStats);
		return enemy;
	}

	public void RecalculateStats()
	{
		combatStats?.Initialize(stats);
	}
}
