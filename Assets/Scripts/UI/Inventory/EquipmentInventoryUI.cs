using System.Collections.Generic;
using UnityEngine;

public class EquipmentInventoryUI : MonoBehaviour
{
    [SerializeField] private EquipmentInventory inventory;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject equipmentSlotPrefab;
    
    public void PopulateInventory()
    {
        IReadOnlyList<Equipment> ownedEquipment = inventory.OwnedEquipment;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (inventory == null)
        {
            inventory = EquipmentInventory.Instance;
        }
        PopulateInventory();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
