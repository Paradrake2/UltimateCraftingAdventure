using UnityEngine;

// this is for the character menu. Will display the ally's total stats from equipment, runes, etc. Does not include buffs/debuffs from current combat
public class AllyStatUI : MonoBehaviour
{
    [SerializeField] private GameObject statUIPrefab;
    [SerializeField] private Transform statUIContainer;

    private void OnEnable()
    {
        GameEventManager.OnAllyInspected += OnAllyInspected;
    }

    private void OnDisable()
    {
        GameEventManager.OnAllyInspected -= OnAllyInspected;
    }

    private void OnAllyInspected(AllyInspectedArgs args) => UpdateStats(args.Ally);

    private void UpdateStats(Ally ally)
    {
        foreach (Transform child in statUIContainer)
            Destroy(child.gameObject);
        if (ally == null || ally.CombatStats == null) return;
        StatCollection stats = ally.Stats;
        var sorted = new System.Collections.Generic.List<StatValue>(stats.Stats);
        sorted.Sort((a, b) =>
        {
            int order = a.Stat.DisplayOrder.CompareTo(b.Stat.DisplayOrder);
            return order != 0 ? order : string.Compare(a.Stat.StatName, b.Stat.StatName, System.StringComparison.Ordinal);
        });
        foreach (StatValue sv in sorted)
            CreateStatUI(sv);
    }

    private void CreateStatUI(StatValue sv)
    {
        if (statUIPrefab == null || statUIContainer == null) return;
        GameObject statUIObj = Instantiate(statUIPrefab, statUIContainer);
        AllyStatCMUIObject uiObject = statUIObj.GetComponent<AllyStatCMUIObject>();
        if (uiObject != null)
            uiObject.Initialize(sv);
    }
}
