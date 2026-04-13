using UnityEngine;

public enum XPDistributionMethod
{
	PerAllyLevel,       // XPReward * ally.Level  (default)
	MaxLevel,           // XPReward * maxAllyLevel / numAllies
	CatchUp,            // XPReward * maxAllyLevel / numAllies, with catch-up bonus for lower-level allies
	Flat,               // XPReward split evenly regardless of level
}

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Enemy")]
public class Enemy : ScriptableObject
{
	[SerializeField] private string enemyName;
	[SerializeField] private Sprite icon;
	[SerializeField] private StatCollection stats = new StatCollection();
	[SerializeField] private EnemyCombat combatStats = new EnemyCombat();
	[SerializeField] private EnemyLootTable lootTable;
	[SerializeField] private float XPReward;
	public string EnemyName => string.IsNullOrWhiteSpace(enemyName) ? name : enemyName;
	public Sprite Icon => icon;
	public StatCollection Stats => stats;
	public EnemyCombat CombatStats => combatStats;
	public EnemyLootTable LootTable => lootTable;
	public float XPRewardAmount => XPReward;


	public void Initialize(string newEnemyName, Sprite newIcon, StatCollection newStats, EnemyLootTable table)
	{
		enemyName = newEnemyName;
		icon = newIcon;
		stats = newStats ?? new StatCollection();
		lootTable = table;
		combatStats ??= new EnemyCombat();
		combatStats.Initialize(stats, this);
	}

	public static Enemy CreateRuntime(Enemy template)
	{
		if (template == null)
		{
			return null;
		}

		Enemy enemy = CreateInstance<Enemy>();
		StatCollection runtimeStats = template.stats != null ? template.stats.Clone() : new StatCollection();
		enemy.Initialize(template.EnemyName, template.icon, runtimeStats, template.lootTable);
		enemy.XPReward = template.XPReward;
		return enemy;
	}

	public void RecalculateStats()
	{
		combatStats?.Initialize(stats, this);
	}
	public void DropLoot(float luck, CombatMap map, int allyLevel)
	{
		Debug.Log($"{EnemyName} is dropping loot...");
		if (lootTable == null) return;

		var loot = lootTable.GetLootDrop();
		if (loot == null)
		{
			Debug.Log("Enemy dropped no loot.");
			return;
		}

		if (loot is ItemQuantity itemQuantity)
		{
			ItemInventory.Instance.AddItem(itemQuantity.Item, itemQuantity.Quantity);
		}
		else if (loot is Equipment baseEquipment)
		{
			EquipmentRarity rarity = LootGeneration.RollRarity(luck);
			EquipmentGenerationModifier modifier = LootGeneration.GetGenerationModifier(map);
			Equipment dropped = EquipmentFactory.GetLootEquipment(baseEquipment, allyLevel, rarity, modifier);
			EquipmentInventory.Instance.AddEquipment(dropped);
			Debug.Log($"{EnemyName} dropped {dropped.EquipmentName} ({rarity})");
		}
	}
	public void AddXPToAllies(Ally[] allies, int maxAllyLevel, XPDistributionMethod method)
	{
		if (allies == null || allies.Length == 0) return;
		Debug.Log($"Distributing {XPReward} XP among {allies.Length} allies using method {method}.");
		switch (method)
		{
			case XPDistributionMethod.PerAllyLevel:
				for (int i = 0; i < allies.Length; i++)
					if (allies[i] != null)
						allies[i].AddXP(XPReward * Mathf.Max(1, allies[i].Level));
				break;

			case XPDistributionMethod.MaxLevel:
				float maxLevelAmount = XPReward * Mathf.Max(1, maxAllyLevel) / allies.Length;
				for (int i = 0; i < allies.Length; i++)
					allies[i]?.AddXP(maxLevelAmount);
				break;

			case XPDistributionMethod.CatchUp:
				float baseXP = XPReward * Mathf.Max(1, maxAllyLevel) / allies.Length;
				for (int i = 0; i < allies.Length; i++)
				{
					if (allies[i] == null) continue;
					float levelDelta = Mathf.Max(0, maxAllyLevel - allies[i].Level);
					allies[i].AddXP(baseXP * (1f + levelDelta * 0.1f));
				}
				break;

			case XPDistributionMethod.Flat:
				float flatAmount = XPReward / allies.Length;
				for (int i = 0; i < allies.Length; i++)
					allies[i]?.AddXP(flatAmount);
				break;
		}
	}
}
