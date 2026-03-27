using System.Collections.Generic;
using UnityEngine;

public class AllyManager : MonoBehaviour
{
    private static AllyManager _instance;
    public Ally currentAlly;
    [SerializeField] private List<Ally> allies = new List<Ally>();
    public static AllyManager Instance => _instance;

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
    public void AddAlly(Ally ally)
    {
        if (ally == null) return;
        if (allies == null) allies = new List<Ally>();
        allies.Add(ally);
    }
    public void RemoveAlly(Ally ally)
    {
        allies.Remove(ally);
        if (currentAlly == ally)
        {
            currentAlly = null;
        }
    }
    public List<Ally> GetAllies()
    {
        return allies;
    }

    void Start()
    {
        
    }
}
