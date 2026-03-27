using UnityEngine;

public class UIHoverManager : MonoBehaviour
{
    [SerializeField] private GameObject tooltipPrefab;
    private static UIHoverManager _instance;
    public static UIHoverManager Instance => _instance;

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
