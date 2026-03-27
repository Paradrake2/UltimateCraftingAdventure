using System.Collections.Generic;
using UnityEngine;

public class EquipmentInventory : MonoBehaviour
{
    private static EquipmentInventory _instance;
    [SerializeField] private List<Equipment> ownedEquipment = new List<Equipment>();
    public IReadOnlyList<Equipment> OwnedEquipment => ownedEquipment;
    public static EquipmentInventory Instance => _instance;

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

    public void AddEquipment(Equipment equipment)
    {
        ownedEquipment.Add(equipment);
    }
    public void RemoveEquipmentByID(string equipmentID)
    {
        ownedEquipment.RemoveAll(e => e.ID == equipmentID);
    }
    public bool TryGetEquipmentByID(string equipmentID, out Equipment equipment)
    {
        equipment = ownedEquipment.Find(e => e.ID == equipmentID);
        return equipment != null;
    }
}
