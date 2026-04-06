using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    [Header("Encounter")]
    [SerializeField] private CombatMap map;
    [SerializeField] private AllyCombatWindowUI allyCombatWindowUI;
    [SerializeField] private EnemyCombatWindowUI enemyCombatWindowUI;
    [SerializeField] private GameObject combatInitializerPrefab;
    [SerializeField] private Ally[] allies;
    [SerializeField] private bool useAlliesFromManager = true;
    [SerializeField] private bool startCombatOnEnable;

    [Header("Runtime")]
    [SerializeField] private List<Enemy> activeEnemies = new List<Enemy>();
    [SerializeField] private bool isCombatActive;
    [SerializeField] private int currentWaveNumber;
    [SerializeField] private int defeatedRegularWaves;
    [SerializeField] private bool bossWaveActive;
    [SerializeField] private bool encounterCompleted;

    private readonly List<Ally> livingAlliesBuffer = new List<Ally>();
    private readonly List<Coroutine> enemyAttackRoutines = new List<Coroutine>();
    private readonly List<Coroutine> allySkillRoutines = new List<Coroutine>();

    public CombatMap Map => map;
    public Ally[] Allies => allies;
    public IReadOnlyList<Enemy> Enemies => activeEnemies;
    public bool IsCombatActive => isCombatActive;
    public int CurrentWaveNumber => currentWaveNumber;
    public int DefeatedRegularWaves => defeatedRegularWaves;
    public bool BossWaveActive => bossWaveActive;
    public bool EncounterCompleted => encounterCompleted;
    public int MaxAlliesInBattle => map != null ? map.MaxAlliesInBattle : Mathf.Max(1, allies != null ? allies.Length : 1);

    private void Awake()
    {
        RefreshAlliesFromManager();
        if (combatInitializerPrefab != null)
            combatInitializerPrefab.SetActive(false);
    }

    private void OnEnable()
    {
        MissionManager.OnMapSelected += OnMapSelected;
        if (startCombatOnEnable)
        {
            StartCombat();
        }
    }

    private void OnDisable()
    {
        MissionManager.OnMapSelected -= OnMapSelected;
    }

    private void OnMapSelected(CombatMap selectedMap)
    {
        if (isCombatActive) return;
        SetMap(selectedMap);
        combatInitializerPrefab?.SetActive(true);
    }
    public void SceneLoaded()
    {
        RefreshAlliesFromManager();
        if (!isCombatActive)
        {
            bool mapSelected = MissionManager.SelectedMap != null;
            if (mapSelected)
                SetMap(MissionManager.SelectedMap);
            combatInitializerPrefab?.SetActive(mapSelected);
        }
        if (startCombatOnEnable && !isCombatActive)
        {
            StartCombat();
        }
    }
    public void StartCombat()
    {
        EndCombat();
        RefreshAlliesFromManager();
        allyCombatWindowUI.ClearAllyCombatInfo();
        enemyCombatWindowUI.ClearEnemyCombatInfo();
        if (combatInitializerPrefab != null)
        {
            combatInitializerPrefab.SetActive(false);
        }
        Debug.Log("Attempting to start combat. Map: " + (map != null ? map.name : "None") + ", Allies: " + (allies != null ? allies.Length.ToString() : "None"));
        if (!CanStartCombat())
        {
            isCombatActive = false;
            return;
        }

        currentWaveNumber = 0;
        defeatedRegularWaves = 0;
        bossWaveActive = false;
        encounterCompleted = false;
        activeEnemies.Clear();

        InitializeAlliesForCombat();
        isCombatActive = true;
        StartAllySkillLoops();
        SpawnNextWave();
    }

    public void EndCombat()
    {
        isCombatActive = false;
        StopAttackLoops(enemyAttackRoutines);
        StopAttackLoops(allySkillRoutines);
    }
    public void ForceEndCombat()
    {
        isCombatActive = false;
        StopAttackLoops(enemyAttackRoutines);
        StopAttackLoops(allySkillRoutines);
        activeEnemies.Clear();
        currentWaveNumber = 0;
        defeatedRegularWaves = 0;
        bossWaveActive = false;
        encounterCompleted = false;
        enemyCombatWindowUI.ClearEnemyCombatInfo();
        allyCombatWindowUI.ClearAllyCombatInfo();
        map = null;
    }
    public void SetMap(CombatMap newMap)
    {
        map = newMap;
    }

    public void SetAllies(Ally[] newAllies)
    {
        allies = newAllies;
    }

    private bool CanStartCombat()
    {
        if (map == null)
        {
            Debug.LogWarning("Cannot start combat without a CombatMap.", this);
            return false;
        }

        if (!HasLivingAlliesConfigured())
        {
            Debug.LogWarning("Cannot start combat without a selected combat party.", this);
            return false;
        }

        if ((map.PossibleEnemies == null || map.PossibleEnemies.Count == 0) && map.BossEnemy == null)
        {
            Debug.LogWarning("Cannot start combat without configured map enemies.", this);
            return false;
        }

        return true;
    }

    private void InitializeAlliesForCombat()
    {
        if (allies == null)
        {
            return;
        }

        for (int i = 0; i < allies.Length; i++)
        {
            Ally ally = allies[i];
            if (ally == null || ally.CombatStats == null)
            {
                continue;
            }

            ally.CombatStats.Initialize(ally.Stats);
            ally.CombatStats.ResetForCombat();
        }
        allyCombatWindowUI.PopulateAllyCombatInfo(allies);
    }

    private void SpawnNextWave()
    {
        if (ShouldSpawnBossWave())
        {
            SpawnBossWave();
            return;
        }

        SpawnRegularWave();
    }

    private bool ShouldSpawnBossWave()
    {
        return !bossWaveActive && map != null && map.BossEnemy != null && defeatedRegularWaves >= map.WavesBeforeBoss;
    }

    private void SpawnRegularWave()
    {
        StopAttackLoops(enemyAttackRoutines);
        activeEnemies.Clear();

        if (map == null)
        {
            EndCombat();
            return;
        }

        int enemyCount = map.MaxActiveEnemies;
        for (int i = 0; i < enemyCount; i++)
        {
            Enemy template = map.GetRandomEnemyTemplate();
            Enemy runtimeEnemy = Enemy.CreateRuntime(template);
            if (runtimeEnemy == null || runtimeEnemy.CombatStats == null)
            {
                continue;
            }

            runtimeEnemy.CombatStats.ResetForCombat();
            activeEnemies.Add(runtimeEnemy);
        }

        if (activeEnemies.Count == 0)
        {
            Debug.LogWarning("Combat map could not spawn a valid regular wave.", this);
            EndCombat();
            return;
        }
        enemyCombatWindowUI.PopulateEnemyCombatInfo(activeEnemies);
        currentWaveNumber++;
        bossWaveActive = false;
        StartEnemyAttackLoops();
    }

    private void SpawnBossWave()
    {
        StopAttackLoops(enemyAttackRoutines);
        activeEnemies.Clear();

        Enemy runtimeBoss = Enemy.CreateRuntime(map.BossEnemy);
        if (runtimeBoss == null || runtimeBoss.CombatStats == null)
        {
            Debug.LogWarning("Combat map could not spawn a valid boss wave.", this);
            EndCombat();
            return;
        }

        runtimeBoss.CombatStats.ResetForCombat();
        activeEnemies.Add(runtimeBoss);
        currentWaveNumber++;
        bossWaveActive = true;
        enemyCombatWindowUI.PopulateEnemyCombatInfo(activeEnemies);

        StartEnemyAttackLoops();
    }

    private void HandleWaveCleared()
    {
        if (bossWaveActive)
        {
            encounterCompleted = true;
            HandleEncounterCompleted();
            return;
        }

        defeatedRegularWaves++;
        SpawnNextWave();
    }
    private void HandleEncounterCompleted()
    {
        if (map == null)
        {
            return;
        }

        foreach (var unlockedMap in map.MapsUnlockedOnCompletion)
        {
            if (unlockedMap != null)
            {
                unlockedMap.Unlock();
            }
        }
        StartCombat();
    }
    private void StartAllySkillLoops()
    {
        StopAttackLoops(allySkillRoutines);

        if (allies == null) return;

        for (int i = 0; i < allies.Length; i++)
        {
            Ally ally = allies[i];
            if (ally == null || ally.CombatStats == null) continue;
            if (ally.CombatStats.SkillSlots == null || ally.CombatStats.SkillSlots.Count == 0) continue;

            allySkillRoutines.Add(StartCoroutine(RunAllySkillLoop(ally)));
        }
    }

    private System.Collections.IEnumerator RunAllySkillLoop(Ally ally)
    {
        const float tickInterval = 0.1f;
        var wait = new WaitForSeconds(tickInterval);

        while (isCombatActive)
        {
            if (ally == null || ally.CombatStats == null || !ally.CombatStats.IsAlive)
            {
                yield break;
            }

            yield return wait;

            if (!isCombatActive || ally.CombatStats == null || !ally.CombatStats.IsAlive)
            {
                yield break;
            }

            FillLivingAlliesBuffer();
            var context = new SkillContext(ally, livingAlliesBuffer, activeEnemies);
            if (ally.CombatStats.TickAndUseSkills(context, tickInterval))
            {
                ResolveCombatState();
            }
        }
    }

    private void StartEnemyAttackLoops()
    {
        StopAttackLoops(enemyAttackRoutines);

        if (activeEnemies.Count == 0)
        {
            return;
        }

        for (int i = 0; i < activeEnemies.Count; i++)
        {
            Enemy enemy = activeEnemies[i];
            if (enemy == null || enemy.CombatStats == null)
            {
                continue;
            }

            enemyAttackRoutines.Add(StartCoroutine(RunEnemyAttackLoop(enemy)));
        }
    }

    private System.Collections.IEnumerator RunEnemyAttackLoop(Enemy enemy)
    {
        while (isCombatActive)
        {
            if (enemy == null || enemy.CombatStats == null || !enemy.CombatStats.IsAlive)
            {
                yield break;
            }

            yield return new WaitForSeconds(enemy.CombatStats.GetAttackInterval());

            if (!isCombatActive || enemy.CombatStats == null || !enemy.CombatStats.IsAlive)
            {
                yield break;
            }

            FillLivingAlliesBuffer();
            if (!enemy.CombatStats.AttackTargets(livingAlliesBuffer))
            {
                continue;
            }

            ResolveCombatState();
        }
    }

    private void ResolveCombatState()
    {
        if (!isCombatActive)
        {
            return;
        }

        RemoveDefeatedEnemies();
        if (AllAlliesDefeated())
        {
            ForceEndCombat();
            return;
        }

        if (activeEnemies.Count == 0)
        {
            HandleWaveCleared();
        }
    }

    private void FillLivingAlliesBuffer()
    {
        livingAlliesBuffer.Clear();

        if (allies == null)
        {
            return;
        }

        for (int i = 0; i < allies.Length; i++)
        {
            Ally ally = allies[i];
            if (ally != null && ally.CombatStats != null && ally.CombatStats.IsAlive)
            {
                livingAlliesBuffer.Add(ally);
            }
        }
    }

    private void StopAttackLoops(List<Coroutine> routines)
    {
        for (int i = 0; i < routines.Count; i++)
        {
            if (routines[i] != null)
            {
                StopCoroutine(routines[i]);
            }
        }

        routines.Clear();
    }

    private void RemoveDefeatedEnemies()
    {
        activeEnemies.RemoveAll(enemy => enemy == null || enemy.CombatStats == null || !enemy.CombatStats.IsAlive);
    }

    private bool AllAlliesDefeated()
    {
        if (allies == null || allies.Length == 0)
        {
            return true;
        }

        for (int i = 0; i < allies.Length; i++)
        {
            Ally ally = allies[i];
            if (ally != null && ally.CombatStats != null && ally.CombatStats.IsAlive)
            {
                return false;
            }
        }

        return true;
    }

    private bool HasLivingAlliesConfigured()
    {
        if (allies == null || allies.Length == 0)
        {
            return false;
        }

        for (int i = 0; i < allies.Length; i++)
        {
            if (allies[i] != null)
            {
                return true;
            }
        }

        return false;
    }

    private void RefreshAlliesFromManager()
    {
        if (!useAlliesFromManager || AllyManager.Instance == null)
        {
            return;
        }

        Ally[] selectedCombatParty = AllyManager.Instance.CombatParty.BuildBattleParty(AllyManager.Instance.GetAllies(), MaxAlliesInBattle);
        if (selectedCombatParty.Length > 0)
        {
            allies = selectedCombatParty;
            return;
        }

        List<Ally> managedAllies = AllyManager.Instance.GetAllies();
        if (managedAllies != null && managedAllies.Count > 0)
        {
            allies = System.Array.Empty<Ally>();
            return;
        }

        allies = System.Array.Empty<Ally>();
    }
}
