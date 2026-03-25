using UnityEngine;

public class UIHoverManager : MonoBehaviour
{
    [SerializeField] private GameObject tooltipPrefab;
    private static UIHoverManager instance;
    public static UIHoverManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<UIHoverManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("UIHoverManager");
                    instance = obj.AddComponent<UIHoverManager>();
                }
            }
            return instance;
        }
    }
    public void DisplayEquipmentTooltip(Equipment equipment, Vector3 position)
    {
        if (tooltipPrefab == null)
        {
            Debug.LogError("Tooltip prefab not assigned in UIHoverManager.");
            return;
        }
        tooltipPrefab.SetActive(true);
        tooltipPrefab.transform.position = position;
        // initialize tooltip with equipment info
    }
    public void HideTooltip()
    {
        if (tooltipPrefab != null)
        {
            tooltipPrefab.SetActive(false);
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
