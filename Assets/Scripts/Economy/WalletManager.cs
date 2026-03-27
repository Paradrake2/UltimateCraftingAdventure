using UnityEngine;

public class WalletManager : MonoBehaviour
{
    private static WalletManager _instance;

    [Header("Optional")]
    [Tooltip("If set, will be used by UI/code that wants to map currency ids back to Currency assets.")]
    [SerializeField] private CurrencyDatabase currencyDatabase;

    public static WalletManager Instance => _instance;

    public Wallet Wallet { get; private set; }
    public CurrencyDatabase CurrencyDb => currencyDatabase != null ? currencyDatabase : CurrencyDatabase.Instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        Wallet = new Wallet();
        Wallet.LoadFrom(WalletPersistence.LoadOrCreate());
        Wallet.BalanceChanged += OnBalanceChanged;
    }

    private void OnDestroy()
    {
        if (Wallet != null)
        {
            Wallet.BalanceChanged -= OnBalanceChanged;
        }
    }

    private void OnBalanceChanged(string currencyId, long newBalance)
    {
        WalletPersistence.TrySave(Wallet.ToSaveData(), out _);
    }

    private void OnApplicationQuit()
    {
        if (Wallet == null) return;
        WalletPersistence.TrySave(Wallet.ToSaveData(), out _);
    }

    public long GetBalance(Currency currency)
    {
        return Wallet != null ? Wallet.GetBalance(currency) : 0;
    }

    public void Earn(Currency currency, long amount)
    {
        Wallet?.Earn(currency, amount);
    }

    public bool CanAfford(Cost cost)
    {
        if (Wallet == null) return false;
        if (cost == null) return true;
        return Wallet.CanAfford(cost.Lines);
    }

    public bool CanAfford(System.Collections.Generic.IReadOnlyList<CostLine> costLines)
    {
        if (Wallet == null) return false;
        return Wallet.CanAfford(costLines);
    }

    public bool TrySpend(Cost cost, out string failureReason)
    {
        if (Wallet == null)
        {
            failureReason = "Wallet not initialized.";
            return false;
        }

        if (cost == null)
        {
            failureReason = null;
            return true;
        }

        return Wallet.TrySpend(cost.Lines, out failureReason);
    }

    public bool TrySpend(System.Collections.Generic.IReadOnlyList<CostLine> costLines, out string failureReason)
    {
        if (Wallet == null)
        {
            failureReason = "Wallet not initialized.";
            return false;
        }

        return Wallet.TrySpend(costLines, out failureReason);
    }
}
