using System.Collections.Generic;
using UnityEngine;

public class AllyManager : MonoBehaviour
{
    private static AllyManager _instance;
    // in combat, current ally is used for the one currently selected
    public Ally currentAlly;
    [SerializeField] private List<Ally> allies = new List<Ally>();
    [SerializeField] private CombatParty combatParty = new CombatParty();
    public static AllyManager Instance => _instance;
    public CombatParty CombatParty => combatParty;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
        combatParty ??= new CombatParty();
    }

    public void SwitchAlly(Ally newAlly)
    {
        currentAlly = newAlly;
        Debug.Log("Switched to ally: " + (currentAlly != null ? currentAlly.name : "None"));
        // load equipment, skills, stats, etc
        PopulateAllyEquipment(currentAlly);
    }
    private void PopulateAllyEquipment(Ally ally)
    {
        if (ally == null) return;
        Debug.Log("Populating equipment for ally: " + ally.name);
        EquipmentHolder.Instance.UI.PopulateSlots(ally.EquipmentInventory);
    }

    public void RefreshEquipmentUI()
    {
        PopulateAllyEquipment(currentAlly);
    }
    public void AddAlly(Ally ally)
    {
        if (ally == null) return;
        if (allies == null) allies = new List<Ally>();
        if (allies.Contains(ally)) return;
        allies.Add(ally);
    }
    public void RemoveAlly(Ally ally)
    {
        allies.Remove(ally);
        combatParty?.Remove(ally);
        if (currentAlly == ally)
        {
            currentAlly = null;
        }
    }
    public List<Ally> GetAllies()
    {
        return allies;
    }

    public void ClearAllies()
    {
        allies.Clear();
        currentAlly = null;
    }

    void Start()
    {
        
    }
}
