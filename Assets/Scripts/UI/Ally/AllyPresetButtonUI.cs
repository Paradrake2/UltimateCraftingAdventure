using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllyPresetButtonUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI label;

    private AllyPreset preset;
    private AllyFactoryUI owner;

    public void Bind(AllyFactoryUI ownerUi, AllyPreset allyPreset)
    {
        owner = ownerUi;
        preset = allyPreset;

        if (label != null)
        {
            label.text = preset != null ? preset.DisplayName : "(Missing Preset)";
        }

        if (iconImage != null)
        {
            var icon = preset != null ? (preset.IconOverride != null ? preset.IconOverride : (preset.Archetype != null ? preset.Archetype.Icon : null)) : null;
            iconImage.sprite = icon;
            iconImage.enabled = icon != null;
        }

        if (button == null) button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.RemoveListener(OnClicked);
            button.onClick.AddListener(OnClicked);
        }
    }

    private void OnClicked()
    {
        if (owner == null || preset == null) return;
        owner.SelectPreset(preset);
    }
}
