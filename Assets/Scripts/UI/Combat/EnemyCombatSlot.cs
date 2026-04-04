using UnityEngine;
using UnityEngine.UI;
public class EnemyCombatSlot : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    [SerializeField] private Image icon;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject attackBar;

    public void Initialize(Enemy newEnemy)
    {
        enemy = newEnemy;
        if (enemy != null)
        {
            enemy.CombatStats.Initialize(enemy.Stats);
            icon.sprite = enemy.Icon;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
