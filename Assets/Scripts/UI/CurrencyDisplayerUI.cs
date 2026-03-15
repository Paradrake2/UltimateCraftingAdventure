using UnityEngine;

public class CurrencyDisplayerUI : MonoBehaviour
{
    private static CurrencyDisplayerUI instance;
    public GameObject goldDisplay;
    public GameObject soulsDisplay;



    public void UpdateCurrencyDisplay()
    {
        var wallet = WalletManager.Instance.Wallet;
        if (wallet != null)
        {
            if (goldDisplay != null)
            {
                var goldText = goldDisplay.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (goldText != null)
                {
                    goldText.text = wallet.GetBalance("gold").ToString();
                }
            }
            if (soulsDisplay != null)
            {
                var soulsText = soulsDisplay.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (soulsText != null)
                {
                    soulsText.text = wallet.GetBalance("souls").ToString();
                }
            }
        }
    }
    public static CurrencyDisplayerUI Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<CurrencyDisplayerUI>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("CurrencyDisplayerUI");
                    instance = obj.AddComponent<CurrencyDisplayerUI>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateCurrencyDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
