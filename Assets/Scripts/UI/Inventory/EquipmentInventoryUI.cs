using System.Collections.Generic;
using UnityEngine;

public class EquipmentInventoryUI : MonoBehaviour
{
    [SerializeField] private EquipmentInventory inventory;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject equipmentSlotPrefab;
    
    public void PopulateInventory()
    {
        foreach (Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (var equipment in inventory.OwnedEquipment)
        {
            GameObject slot = Instantiate(equipmentSlotPrefab, inventoryPanel.transform);
            EquipmentInventorySlotUI slotUI = slot.GetComponent<EquipmentInventorySlotUI>();
            if (slotUI != null)
            {
                slotUI.Initialize(equipment);
            }
        }

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (inventory == null)
        {
            Debug.LogError("EquipmentInventory not assigned. Please assign the EquipmentInventory asset in the Inspector.");
            return;
        }
        PopulateInventory();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
