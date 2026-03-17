using System;
using UnityEngine;

public class EquipmentHolderUI : MonoBehaviour
{
    [SerializeField] private GameObject helmetSlot;
    [SerializeField] private GameObject chestplateSlot;
    [SerializeField] private GameObject leggingsSlot;
    [SerializeField] private GameObject bootsSlot;
    [SerializeField] private GameObject glovesSlot;
    [SerializeField] private GameObject bracersSlot;
    [SerializeField] private GameObject handSlot1;
    [SerializeField] private GameObject handSlot2;
    [SerializeField] private GameObject accessorySlot1;
    [SerializeField] private GameObject accessorySlot2;



    public void PopulateSlots(AllyEquipmentInventory inventory)
    {
        PopulateSlot(helmetSlot, inventory.Helmet);
        PopulateSlot(chestplateSlot, inventory.Chestplate);
        PopulateSlot(leggingsSlot, inventory.Leggings);
        PopulateSlot(bootsSlot, inventory.Boots);
        PopulateSlot(glovesSlot, inventory.Gloves);
        PopulateSlot(bracersSlot, inventory.Bracers);
        PopulateSlot(handSlot1, inventory.HandSlot1);
        PopulateSlot(handSlot2, inventory.HandSlot2);
        PopulateSlot(accessorySlot1, inventory.AccessorySlot1);
        PopulateSlot(accessorySlot2, inventory.AccessorySlot2);
    }

    private void PopulateSlot(GameObject slot, Equipment equipment)
    {
        if (equipment!= null) slot.GetComponent<EquipmentSlotUI>().SetEquipment(equipment);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
