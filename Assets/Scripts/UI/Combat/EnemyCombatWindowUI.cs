using UnityEngine;

public class EnemyCombatWindowUI : MonoBehaviour
{
    [SerializeField] private Transform enemyCombatWindowContent;
    [SerializeField] private GameObject enemyCombatInfoPrefab;

    public void PopulateEnemyCombatInfo(Enemy[] enemies)
    {
        if (enemyCombatWindowContent == null || enemyCombatInfoPrefab == null)
        {
            return;
        }

        foreach (Transform child in enemyCombatWindowContent)
        {
            Destroy(child.gameObject);
        }

        if (enemies == null)
        {
            return;
        }

        foreach (Enemy enemy in enemies)
        {
            GameObject infoObj = Instantiate(enemyCombatInfoPrefab, enemyCombatWindowContent);
            EnemyCombatSlot slot = infoObj.GetComponent<EnemyCombatSlot>();
            if (slot != null)
            {
                slot.Initialize(enemy);
            }
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
