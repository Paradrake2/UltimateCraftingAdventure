using UnityEngine;
using UnityEngine.UI;

public class AllySelectButton : MonoBehaviour
{
    [SerializeField] private Ally ally;
    [SerializeField] private Image icon;
    public void OnClick()
    {
        if (ally != null)
        {
            AllyManager.Instance.SwitchAlly(ally);
        }
    }
    public void Initialize(Ally ally)
    {
        this.ally = ally;
        if (icon != null)
        {
            icon.sprite = ally != null ? ally.Icon : null;
            icon.enabled = ally != null && ally.Icon != null;
        }
    }
}
