using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentInventory", menuName = "Inventory/Equipment Inventory")]
public class EquipmentInventory : ScriptableObject
{
    [SerializeField] private List<Equipment> ownedEquipment = new List<Equipment>();
    [System.NonSerialized] private List<Equipment> runtimeEquipment;
    public IReadOnlyList<Equipment> OwnedEquipment => runtimeEquipment;
    private static EquipmentInventory instance;
    public static EquipmentInventory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<EquipmentInventory>("EquipmentInventory");
                if (instance == null)
                {
                    Debug.LogError("EquipmentInventory asset not found in Resources folder. Please create an EquipmentInventory asset and place it in the Resources folder.");
                }
            }
            return instance;
        }
    }

    private void OnEnable()
    {
        runtimeEquipment = new List<Equipment>(ownedEquipment ?? new List<Equipment>());
    }

    public void AddEquipment(Equipment equipment)
    {
        runtimeEquipment.Add(equipment);
    }
    public void RemoveEquipmentByID(string equipmentID)
    {
        runtimeEquipment.RemoveAll(e => e.ID == equipmentID);
    }
    public bool TryGetEquipmentByID(string equipmentID, out Equipment equipment)
    {
        equipment = runtimeEquipment.Find(e => e.ID == equipmentID);
        return equipment != null;
    }
    public void RemoveEquipment(Equipment equipment)
    {
        runtimeEquipment.Remove(equipment);
    }

    public void Clear()
    {
        runtimeEquipment.Clear();
    }
}
