using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllyFactoryUI : MonoBehaviour
{
    [SerializeField] private AllyManager allyManager;

    [Header("Open/Close")]
    [SerializeField] private GameObject menuRoot;

    [Header("Preset List")]
    [SerializeField] private GameObject presetListRoot;
    [SerializeField] private Transform presetListContent;
    [SerializeField] private AllyPresetButtonUI presetButtonPrefab;
    [SerializeField] private List<AllyPreset> presets = new List<AllyPreset>();

    [Header("Name Entry")]
    [SerializeField] private GameObject nameEntryRoot;
    [SerializeField] private TextMeshProUGUI selectedPresetLabel;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button backButton;

    private AllyPreset selectedPreset;

    // Start is called before the first frame update
    void Start()
    {
        if (allyManager == null)
        {
            allyManager = FindFirstObjectByType<AllyManager>();
            if (allyManager == null)
            {
                Debug.LogError("No AllyManager found in the scene.");
            }
        }

        if (submitButton != null) submitButton.onClick.AddListener(Submit);
        if (backButton != null) backButton.onClick.AddListener(ShowPresetList);

        CloseMenu();
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenMenu()
    {
        if (menuRoot != null) menuRoot.SetActive(true);
        ShowPresetList();
        RefreshPresetList();
    }

    public void CloseMenu()
    {
        selectedPreset = null;
        if (menuRoot != null) menuRoot.SetActive(false);
        if (nameInput != null) nameInput.text = string.Empty;
    }

    private void ShowPresetList()
    {
        selectedPreset = null;
        if (presetListRoot != null) presetListRoot.SetActive(true);
        if (nameEntryRoot != null) nameEntryRoot.SetActive(false);
        if (nameInput != null) nameInput.text = string.Empty;
    }

    public void SelectPreset(AllyPreset preset)
    {
        if (preset == null) return;
        selectedPreset = preset;

        if (presetListRoot != null) presetListRoot.SetActive(false);
        if (nameEntryRoot != null) nameEntryRoot.SetActive(true);

        if (selectedPresetLabel != null)
        {
            selectedPresetLabel.text = preset.DisplayName;
        }

        if (nameInput != null)
        {
            nameInput.text = string.Empty;
            nameInput.placeholder?.GetComponent<TextMeshProUGUI>()?.SetText(preset.DefaultName);
            nameInput.ActivateInputField();
        }
    }

    private void RefreshPresetList()
    {
        if (presetListContent == null || presetButtonPrefab == null) return;

        for (int i = presetListContent.childCount - 1; i >= 0; i--)
        {
            Destroy(presetListContent.GetChild(i).gameObject);
        }

        if (presets == null) return;

        for (int i = 0; i < presets.Count; i++)
        {
            var preset = presets[i];
            if (preset == null) continue;

            var button = Instantiate(presetButtonPrefab, presetListContent);
            button.Bind(this, preset);
        }
    }

    private void Submit()
    {
        if (selectedPreset == null)
        {
            Debug.LogWarning("Cannot create ally: no preset selected.");
            return;
        }

        string chosenName = nameInput != null ? nameInput.text : null;

        if (!selectedPreset.TryCreate(chosenName, out var ally, out var failureReason))
        {
            Debug.LogWarning(failureReason);
            return;
        }

        if (allyManager == null) allyManager = AllyManager.Instance;
        allyManager.AddAlly(ally);
        allyManager.SwitchAlly(ally);
        CloseMenu();
        FindAnyObjectByType<AllyListContainer>().PopulateAllyList();
    }
}
