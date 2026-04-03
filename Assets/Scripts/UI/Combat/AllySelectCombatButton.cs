using UnityEngine;
using UnityEngine.UI;
public class AllySelectCombatButton : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image background;
    [SerializeField] private Ally ally;
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Color unselectedColor = Color.white;
    private bool boolClick =false;
    public void Initialize(Ally newAlly)
    {
        ally = newAlly;
        if (icon != null)
        {
            icon.sprite = ally != null ? ally.Icon : null;
            icon.enabled = ally != null && ally.Icon != null;
        }
    }
    
    
    public void SetIcon(Sprite sprite)
    {
        icon.sprite = sprite;
    }
    public void OnClick()
    {
        if (ally != null)
        {
            if (AllyManager.Instance == null)
            {
                return;
            }

            if (!AllyManager.Instance.CombatParty.ToggleSelection(ally, AllyManager.Instance.GetAllies(), 1, out _, out string failureReason) && !string.IsNullOrWhiteSpace(failureReason))
            {
                Debug.LogWarning(failureReason, this);
            }
            boolClick = !boolClick;
            SetColor(boolClick);
        }
    }
    private void SetColor(bool isSelected)
    {
        if (background != null)
        {
            background.color = isSelected ? selectedColor : unselectedColor;
        }
    }
    void Start()
    {
        SetColor(boolClick);
    }


}
