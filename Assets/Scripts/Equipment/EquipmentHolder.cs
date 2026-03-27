using UnityEngine;

public class EquipmentHolder : MonoBehaviour
{
    [SerializeField] private EquipmentHolderUI ui;
    private static EquipmentHolder _instance;
    public static EquipmentHolder Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        if (ui == null)
        {
            ui = FindFirstObjectByType<EquipmentHolderUI>();
            if (ui == null)
            {
                Debug.LogError("No EquipmentHolderUI found in the scene.");
            }
        }
    }
    public EquipmentHolderUI UI => ui;
    public void Equip(Equipment equipment)
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
