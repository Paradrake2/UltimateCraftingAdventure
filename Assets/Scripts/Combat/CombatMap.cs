using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CombatMap", menuName = "Combat/Map")]
public class CombatMap : ScriptableObject
{
    [SerializeField] private string mapName;
    [SerializeField] private List<Enemy> possibleEnemies = new List<Enemy>();
    [SerializeField] private Enemy bossEnemy;
    [SerializeField] private int maxAlliesInBattle = 3;
    [SerializeField] private int maxActiveEnemies = 4;
    [SerializeField] private int wavesBeforeBoss = 5;
    [SerializeField] private Sprite mapBackground;
    [SerializeField] private List<EquipmentLootTableEntry> lootTableEntries = new List<EquipmentLootTableEntry>();
    [SerializeField] private bool isLocked = true;
    [SerializeField] private List<CombatMap> prerequisiteMaps = new List<CombatMap>();
    [SerializeField] private List<CombatMap> mapsUnlockedOnCompletion = new List<CombatMap>();
    [SerializeField] private XPDistributionMethod xpDistributionMethod = XPDistributionMethod.PerAllyLevel;
    [SerializeField] private LootGenerationModifierChance[] lootGenerationModifierChances;
    [SerializeField] private float luck = 0f;

    public string MapName => string.IsNullOrWhiteSpace(mapName) ? name : mapName;
    public XPDistributionMethod XPDistribution => xpDistributionMethod;
    public float Luck => luck;
    public IReadOnlyList<Enemy> PossibleEnemies => possibleEnemies;
    public Enemy BossEnemy => bossEnemy;
    public int MaxAlliesInBattle => Mathf.Max(1, maxAlliesInBattle);
    public int MaxActiveEnemies => Mathf.Max(1, maxActiveEnemies);
    public int WavesBeforeBoss => Mathf.Max(1, wavesBeforeBoss);
    public Sprite MapBackground => mapBackground;
    public IReadOnlyList<EquipmentLootTableEntry> LootTableEntries => lootTableEntries;
    public bool IsLocked => isLocked;
    public IReadOnlyList<CombatMap> PrerequisiteMaps => prerequisiteMaps;
    public IReadOnlyList<CombatMap> MapsUnlockedOnCompletion => mapsUnlockedOnCompletion;
    public IReadOnlyList<LootGenerationModifierChance> LootGenerationModifierChances => lootGenerationModifierChances;
    public Enemy GetRandomEnemyTemplate()
    {
        if (possibleEnemies == null || possibleEnemies.Count == 0)
        {
            return null;
        }

        List<Enemy> validEnemies = new List<Enemy>();
        for (int i = 0; i < possibleEnemies.Count; i++)
        {
            if (possibleEnemies[i] != null)
            {
                validEnemies.Add(possibleEnemies[i]);
            }
        }

        if (validEnemies.Count == 0)
        {
            return null;
        }

        return validEnemies[Random.Range(0, validEnemies.Count)];
    }
    public Equipment GetEquipmentDrop()
    {
        if (lootTableEntries == null || lootTableEntries.Count == 0)
        {
            return null;
        }

        float totalWeight = 0f;
        foreach (var entry in lootTableEntries)
        {
            totalWeight += entry.dropChance;
        }

        float randomValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0f;

        foreach (var entry in lootTableEntries)
        {
            cumulativeWeight += entry.dropChance;
            if (randomValue <= cumulativeWeight)
            {
                return entry.equipment;
            }
        }

        return null; // Fallback, should not reach here if weights are set correctly
    }
    public void Unlock()
    {
        isLocked = false;
    }
    public bool CanUnlock()
    {
        if (!isLocked) return false; // Already unlocked

        foreach (var prerequisite in prerequisiteMaps)
        {
            if (prerequisite != null && prerequisite.IsLocked)
            {
                return false; // At least one prerequisite is still locked
            }
        }

        return true; // All prerequisites are unlocked
    }
}