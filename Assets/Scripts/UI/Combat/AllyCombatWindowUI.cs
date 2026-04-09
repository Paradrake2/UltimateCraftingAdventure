using UnityEngine;

public class AllyCombatWindowUI : MonoBehaviour
{
    [SerializeField] private Transform allyWindowContent;
    [SerializeField] private GameObject allyCombatInfoPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PopulateAllyCombatInfo(Ally[] allies)
    {
        if (allyWindowContent == null || allyCombatInfoPrefab == null)
        {
            return;
        }

        foreach (Transform child in allyWindowContent)
        {
            Destroy(child.gameObject);
        }

        if (allies == null)
        {
            return;
        }

        foreach (Ally ally in allies)
        {
            GameObject infoObj = Instantiate(allyCombatInfoPrefab, allyWindowContent);
            AllyCombatSlot slot = infoObj.GetComponent<AllyCombatSlot>();
            if (slot != null)
            {
                slot.Initialize(ally);
            }
        }
    }
    public void ClearAllyCombatInfo()
    {
        if (allyWindowContent == null)
        {
            return;
        }

        foreach (Transform child in allyWindowContent)
        {
            Destroy(child.gameObject);
        }
    }
}
