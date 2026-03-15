using System.Collections.Generic;
using UnityEngine;

public class EquipmentInventory : MonoBehaviour
{
    private static EquipmentInventory instance;
    [SerializeField] private List<Equipment> ownedEquipment = new List<Equipment>();
    public IReadOnlyList<Equipment> OwnedEquipment => ownedEquipment;
    public static EquipmentInventory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<EquipmentInventory>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("EquipmentInventory");
                    instance = obj.AddComponent<EquipmentInventory>();
                }
            }
            return instance;
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
