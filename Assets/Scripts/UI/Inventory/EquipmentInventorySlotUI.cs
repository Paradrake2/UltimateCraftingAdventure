using UnityEngine;
using UnityEngine.UI;

public class EquipmentInventorySlotUI : MonoBehaviour
{
    [SerializeField] private Equipment equipment;
    [SerializeField] private Image icon;
    [SerializeField] private Sprite baseSprite;
    
    public void Initialize(Equipment equipment)
    {
        SetEquipment(equipment);
    }
    public void SetEquipment(Equipment equipment)
    {
        this.equipment = equipment;
        icon.sprite = equipment != null ? equipment.Icon : baseSprite;
    }
    public Equipment GetEquipment()
    {
        return equipment;
    }
    public void ClearEquipment()
    {
        equipment = null;
        icon.sprite = baseSprite;
    }
    public void OnClick()
    {
        AllyManager.Instance.currentAlly.EquipmentInventory.TryEquip(equipment, out _);
    }
}
