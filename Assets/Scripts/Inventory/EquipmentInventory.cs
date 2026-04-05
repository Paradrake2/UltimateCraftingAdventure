using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentInventory", menuName = "Inventory/Equipment Inventory")]
public class EquipmentInventory : ScriptableObject
{
    [SerializeField] private List<Equipment> ownedEquipment = new List<Equipment>();
    public IReadOnlyList<Equipment> OwnedEquipment => ownedEquipment;


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
    public void RemoveEquipment(Equipment equipment)
    {
        ownedEquipment.Remove(equipment);
    }
}
