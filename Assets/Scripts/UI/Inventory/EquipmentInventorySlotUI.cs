using UnityEngine;

public class EquipmentInventorySlotUI : MonoBehaviour
{
    [SerializeField] private Equipment equipment;
    [SerializeField] private Sprite sprite;
    [SerializeField] private Sprite baseSprite;
    
    public void Initialize(Equipment equipment)
    {
        SetEquipment(equipment);
    }
    public void SetEquipment(Equipment equipment)
    {
        this.equipment = equipment;
        sprite = equipment != null ? equipment.Icon : baseSprite;
    }
    public Equipment GetEquipment()
    {
        return equipment;
    }
    public void ClearEquipment()
    {
        equipment = null;
        sprite = baseSprite;
    }
    public void OnClick()
    {
        AllyManager.Instance.currentAlly.EquipmentInventory.TryEquip(equipment, out _);
    }
}
