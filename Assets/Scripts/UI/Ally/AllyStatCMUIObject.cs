using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AllyStatCMUIObject : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statNameText;
    [SerializeField] private TextMeshProUGUI statValueText;
    [SerializeField] private Image statIcon;

    public void Initialize(StatValue sv)
    {
        if (statNameText != null)
        {
            statNameText.text = sv.Stat.StatName;
            statNameText.color = sv.Stat.StatType switch
            {
                StatType.Damaging => Color.red,
                StatType.Defensive => Color.blue,
                StatType.Resistance => Color.green,
                _ => statNameText.color
            };
        }
        if (statValueText != null)
            statValueText.text = sv.Value.ToString("F0");
        if (statIcon != null)
            statIcon.sprite = sv.Stat.Icon;
    }
}
