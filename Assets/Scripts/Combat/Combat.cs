using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    [Header("Encounter")]
    [SerializeField] private CombatMap map;
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
    private readonly List<Coroutine> allyAttackRoutines = new List<Coroutine>();
    private readonly List<Coroutine> enemyAttackRoutines = new List<Coroutine>();

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
    }

    private void OnEnable()
    {
        if (startCombatOnEnable)
        {
            StartCombat();
        }
    }

    public void StartCombat()
    {
        EndCombat();
        RefreshAlliesFromManager();

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
        StartAllyAttackLoops();
        SpawnNextWave();
    }

    public void EndCombat()
    {
        isCombatActive = false;
        StopAttackLoops(allyAttackRoutines);
        StopAttackLoops(enemyAttackRoutines);
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
        StartEnemyAttackLoops();
    }

    private void HandleWaveCleared()
    {
        if (bossWaveActive)
        {
            encounterCompleted = true;
            EndCombat();
            return;
        }

        defeatedRegularWaves++;
        SpawnNextWave();
    }

    private void StartAllyAttackLoops()
    {
        StopAttackLoops(allyAttackRoutines);

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

            allyAttackRoutines.Add(StartCoroutine(RunAllyAttackLoop(ally)));
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

    private System.Collections.IEnumerator RunAllyAttackLoop(Ally ally)
    {
        while (isCombatActive)
        {
            if (ally == null || ally.CombatStats == null || !ally.CombatStats.IsAlive)
            {
                yield break;
            }

            yield return new WaitForSeconds(ally.CombatStats.GetAttackInterval());

            if (!isCombatActive || ally.CombatStats == null || !ally.CombatStats.IsAlive)
            {
                yield break;
            }

            Enemy target = ally.CombatStats.GetAttackTarget(activeEnemies);
            if (target == null)
            {
                continue;
            }

            ally.CombatStats.Attack(target);
            ResolveCombatState();
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
            Ally target = enemy.CombatStats.GetAttackTarget(livingAlliesBuffer);
            if (target == null)
            {
                continue;
            }

            enemy.CombatStats.Attack(target);
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
            EndCombat();
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
