using UnityEngine;

public class AllyListContainer : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject allySelectButtonPrefab;
    [SerializeField] private bool useForCombatSelection;
    [SerializeField] private Combat combat;
    [SerializeField] private int maxCombatPartySizeOverride;

    private void OnEnable()
    {
        PopulateAllyList();
    }

    public void PopulateAllyList()
    {
        if (content == null || allySelectButtonPrefab == null || AllyManager.Instance == null)
        {
            return;
        }

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        int maxCombatPartySize = GetMaxCombatPartySize();
        foreach (Ally ally in AllyManager.Instance.GetAllies())
        {
            GameObject buttonObj = Instantiate(allySelectButtonPrefab, content);
            AllySelectButton button = buttonObj.GetComponent<AllySelectButton>();
            if (button != null)
            {
                button.Initialize(ally, useForCombatSelection, maxCombatPartySize);
            }
        }
    }

    private int GetMaxCombatPartySize()
    {
        if (maxCombatPartySizeOverride > 0)
        {
            return maxCombatPartySizeOverride;
        }

        if (combat != null)
        {
            return combat.MaxAlliesInBattle;
        }

        return 1;
    }
}
