using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CurrencyDatabase", menuName = "Scriptable Objects/Economy/Currency Database")]
public class CurrencyDatabase : ScriptableObject
{
    [SerializeField] private List<Currency> currencies = new List<Currency>();

    private Dictionary<string, Currency> byId;

    public IReadOnlyList<Currency> Currencies => currencies;

    private void OnEnable()
    {
        RebuildIndex();
    }

    private void RebuildIndex()
    {
        byId = new Dictionary<string, Currency>(StringComparer.Ordinal);
        if (currencies == null) return;

        foreach (var currency in currencies)
        {
            if (currency == null) continue;
            if (string.IsNullOrWhiteSpace(currency.Id)) continue;
            byId[currency.Id] = currency;
        }
    }

    public bool TryGetById(string currencyId, out Currency currency)
    {
        currency = null;
        if (string.IsNullOrWhiteSpace(currencyId)) return false;
        if (byId == null) RebuildIndex();
        return byId.TryGetValue(currencyId, out currency);
    }

    private static CurrencyDatabase instance;
    public static CurrencyDatabase Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<CurrencyDatabase>("CurrencyDatabase");
                if (instance == null)
                {
                    Debug.LogError("CurrencyDatabase asset not found in Resources folder.");
                }
            }
            return instance;
        }
    }
}
