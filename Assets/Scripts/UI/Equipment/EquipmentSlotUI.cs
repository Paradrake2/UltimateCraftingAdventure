using UnityEngine;

public class EquipmentSlotUI : MonoBehaviour
{
    [SerializeField] private Equipment equipment;
    [SerializeField] private Sprite sprite;
    [SerializeField] private Sprite baseSprite;
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

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
