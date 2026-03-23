using UnityEngine;

public class EquipmentHolder : MonoBehaviour
{
    [SerializeField] private EquipmentHolderUI ui;
    private static EquipmentHolder instance;
    public static EquipmentHolder Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<EquipmentHolder>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("EquipmentHolder");
                    instance = obj.AddComponent<EquipmentHolder>();
                }
            }
            return instance;
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
