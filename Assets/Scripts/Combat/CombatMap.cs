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

    public string MapName => string.IsNullOrWhiteSpace(mapName) ? name : mapName;
    public IReadOnlyList<Enemy> PossibleEnemies => possibleEnemies;
    public Enemy BossEnemy => bossEnemy;
    public int MaxAlliesInBattle => Mathf.Max(1, maxAlliesInBattle);
    public int MaxActiveEnemies => Mathf.Max(1, maxActiveEnemies);
    public int WavesBeforeBoss => Mathf.Max(1, wavesBeforeBoss);

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
}