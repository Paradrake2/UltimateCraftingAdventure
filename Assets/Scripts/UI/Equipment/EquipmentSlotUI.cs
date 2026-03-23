using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour
{
    [SerializeField] private Equipment equipment;
    [SerializeField] private Sprite sprite;
    [SerializeField] private Sprite baseSprite;
    public void SetEquipment(Equipment equipment)
    {
        this.equipment = equipment;
        GetComponent<Image>().sprite = equipment != null ? equipment.Icon : baseSprite;
    }
    public Equipment GetEquipment()
    {
        return equipment;
    }
    public void ClearEquipment()
    {
        equipment = null;
        GetComponent<Image>().sprite = baseSprite;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
