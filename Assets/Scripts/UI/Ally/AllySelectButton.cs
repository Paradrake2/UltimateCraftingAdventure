using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class AllySelectButton : MonoBehaviour
{
    [SerializeField] private Ally ally;
    [SerializeField] private Image icon;
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Color unselectedColor = Color.white;
    [SerializeField] private TextMeshProUGUI nameLabel;

    private bool useForCombatSelection;
    private int maxCombatPartySize = 1;

    public void OnClick()
    {
        if (ally != null)
        {
            if (useForCombatSelection)
            {
                if (AllyManager.Instance == null)
                {
                    return;
                }

                if (!AllyManager.Instance.CombatParty.ToggleSelection(ally, AllyManager.Instance.GetAllies(), maxCombatPartySize, out _, out string failureReason) && !string.IsNullOrWhiteSpace(failureReason))
                {
                    Debug.LogWarning(failureReason, this);
                }

                RefreshVisualState();
                return;
            }

            AllyManager.Instance.SwitchAlly(ally);
        }
    }

    public void Initialize(Ally ally, bool useForCombatSelection = false, int maxCombatPartySize = 1)
    {
        this.ally = ally;
        this.useForCombatSelection = useForCombatSelection;
        this.maxCombatPartySize = Mathf.Max(1, maxCombatPartySize);

        if (icon != null)
        {
            icon.sprite = ally != null ? ally.Icon : null;
            icon.enabled = ally != null && ally.Icon != null;
        }
        if (nameLabel != null)
        {
            nameLabel.text = ally != null ? ally.name : "Empty Slot";
        }
        RefreshVisualState();
    }

    private void RefreshVisualState()
    {
        if (icon == null)
        {
            return;
        }

        if (!useForCombatSelection || AllyManager.Instance == null)
        {
            icon.color = unselectedColor;
            return;
        }

        icon.color = AllyManager.Instance.CombatParty.Contains(ally) ? selectedColor : unselectedColor;
    }
}
