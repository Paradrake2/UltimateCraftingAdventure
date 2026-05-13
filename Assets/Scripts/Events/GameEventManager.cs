using System;
using UnityEngine;

// ─── Event argument structs ───────────────────────────────────────────────────

public readonly struct CombatStartedArgs
{
    public readonly Ally[] Allies;
    public CombatStartedArgs(Ally[] allies) => Allies = allies;
}

public readonly struct CombatEndedArgs
{
    /// <summary>True when the encounter was completed; false when all allies were defeated.</summary>
    public readonly bool AlliesWon;
    public CombatEndedArgs(bool alliesWon) => AlliesWon = alliesWon;
}

public readonly struct AllyDiedArgs
{
    public readonly Ally Ally;
    /// <summary>The enemy that dealt the killing blow. May be null.</summary>
    public readonly Enemy Attacker;
    public AllyDiedArgs(Ally ally, Enemy attacker) { Ally = ally; Attacker = attacker; }
}

public readonly struct EnemyDiedArgs
{
    public readonly Enemy Enemy;
    public EnemyDiedArgs(Enemy enemy) => Enemy = enemy;
}

public readonly struct WaveClearedArgs
{
    public readonly int WaveNumber;
    public readonly bool WasBossWave;
    public WaveClearedArgs(int waveNumber, bool wasBossWave) { WaveNumber = waveNumber; WasBossWave = wasBossWave; }
}

public readonly struct AllyInspectedArgs
{
    /// <summary>The ally now being viewed. Null means no ally is selected.</summary>
    public readonly Ally Ally;
    public AllyInspectedArgs(Ally ally) => Ally = ally;
}

// ─── Manager ──────────────────────────────────────────────────────────────────

/// <summary>
/// Central event hub for game-wide events. Place one instance in your
/// persistent scene alongside <see cref="GameSaveManager"/>.
///
/// <para>Subscribe using <c>+=</c> and always unsubscribe with <c>-=</c> when
/// the subscriber is destroyed to avoid memory leaks, since the events are
/// static and survive scene loads.</para>
/// </summary>
public class GameEventManager : MonoBehaviour
{
    private static GameEventManager _instance;
    public static GameEventManager Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ─── Events ───────────────────────────────────────────────────────────────

    public static event Action<CombatStartedArgs> OnCombatStarted;
    public static event Action<CombatEndedArgs>   OnCombatEnded;
    public static event Action<AllyDiedArgs>       OnAllyDied;
    public static event Action<EnemyDiedArgs>      OnEnemyDied;
    public static event Action<WaveClearedArgs>    OnWaveCleared;
    /// <summary>Fired whenever the inspected ally changes or their stats may have changed (equip, unequip, level-up, etc.).</summary>
    public static event Action<AllyInspectedArgs>  OnAllyInspected;

    // ─── Fire helpers (called internally by the combat system) ───────────────

    internal static void FireCombatStarted(CombatStartedArgs args) => OnCombatStarted?.Invoke(args);
    internal static void FireCombatEnded(CombatEndedArgs args)     => OnCombatEnded?.Invoke(args);
    internal static void FireAllyDied(AllyDiedArgs args)           => OnAllyDied?.Invoke(args);
    internal static void FireEnemyDied(EnemyDiedArgs args)         => OnEnemyDied?.Invoke(args);
    internal static void FireWaveCleared(WaveClearedArgs args)     => OnWaveCleared?.Invoke(args);
    public static void FireAllyInspected(Ally ally)                => OnAllyInspected?.Invoke(new AllyInspectedArgs(ally));
}
