using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class EquipmentSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Equipment equipment;
    [SerializeField] private Sprite sprite;
    [SerializeField] private Sprite baseSprite;
    public void SetEquipment(Equipment equipment = null)
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
    public void OnPointerEnter(PointerEventData eventData)
    {
        // show tooltip with equipment info
        UIHoverManager.Instance.DisplayEquipmentTooltip(equipment, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // hide tooltip
        UIHoverManager.Instance.HideTooltip();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
