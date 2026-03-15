using System;
using System.Collections.Generic;

public sealed class Wallet
{
    private readonly Dictionary<string, long> balancesByCurrencyId = new Dictionary<string, long>(StringComparer.Ordinal);

    public event Action<string, long> BalanceChanged;

    public long GetBalance(Currency currency)
    {
        if (currency == null) return 0;
        return GetBalance(currency.Id);
    }

    public long GetBalance(string currencyId)
    {
        if (string.IsNullOrWhiteSpace(currencyId)) return 0;
        return balancesByCurrencyId.TryGetValue(currencyId, out var value) ? value : 0;
    }

    public bool CanAfford(IReadOnlyList<CostLine> costLines)
    {
        if (costLines == null || costLines.Count == 0) return true;

        var totals = BuildTotals(costLines, out _);
        foreach (var pair in totals)
        {
            if (GetBalance(pair.Key) < pair.Value) return false;
        }

        return true;
    }

    public bool TrySpend(IReadOnlyList<CostLine> costLines, out string failureReason)
    {
        failureReason = null;

        if (costLines == null || costLines.Count == 0) return true;

        var totals = BuildTotals(costLines, out failureReason);
        if (totals == null) return false;

        foreach (var pair in totals)
        {
            if (GetBalance(pair.Key) < pair.Value)
            {
                failureReason = "Cannot afford cost.";
                return false;
            }
        }

        foreach (var pair in totals)
        {
            SetBalance(pair.Key, GetBalance(pair.Key) - pair.Value);
        }

        return true;
    }

    public void Earn(Currency currency, long amount)
    {
        if (currency == null) return;
        Earn(currency.Id, amount);
    }

    public void Earn(string currencyId, long amount)
    {
        if (string.IsNullOrWhiteSpace(currencyId)) return;
        if (amount <= 0) return;

        long current = GetBalance(currencyId);
        long updated;

        try
        {
            checked
            {
                updated = current + amount;
            }
        }
        catch (OverflowException)
        {
            updated = long.MaxValue;
        }

        SetBalance(currencyId, updated);
    }

    public WalletSaveData ToSaveData()
    {
        var data = new WalletSaveData();
        foreach (var pair in balancesByCurrencyId)
        {
            data.balances.Add(new WalletSaveData.BalanceEntry
            {
                currencyId = pair.Key,
                amount = pair.Value
            });
        }
        return data;
    }

    public void LoadFrom(WalletSaveData data)
    {
        balancesByCurrencyId.Clear();

        if (data?.balances == null) return;

        for (int i = 0; i < data.balances.Count; i++)
        {
            var entry = data.balances[i];
            if (entry == null) continue;
            if (string.IsNullOrWhiteSpace(entry.currencyId)) continue;
            if (entry.amount < 0) entry.amount = 0;
            balancesByCurrencyId[entry.currencyId] = entry.amount;
        }
    }

    private void SetBalance(string currencyId, long amount)
    {
        if (string.IsNullOrWhiteSpace(currencyId)) return;
        if (amount < 0) amount = 0;

        balancesByCurrencyId[currencyId] = amount;
        BalanceChanged?.Invoke(currencyId, amount);
    }

    private Dictionary<string, long> BuildTotals(IReadOnlyList<CostLine> lines, out string failureReason)
    {
        failureReason = null;

        var totals = new Dictionary<string, long>(StringComparer.Ordinal);

        for (int i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            if (line.Currency == null)
            {
                failureReason = "Invalid cost: currency is null.";
                return null;
            }

            string currencyId = line.Currency.Id;
            if (string.IsNullOrWhiteSpace(currencyId))
            {
                failureReason = "Invalid cost: currency id is missing.";
                return null;
            }

            if (line.Amount <= 0) continue;

            if (!totals.TryGetValue(currencyId, out var existing)) existing = 0;

            long updated;
            try
            {
                checked
                {
                    updated = existing + line.Amount;
                }
            }
            catch (OverflowException)
            {
                updated = long.MaxValue;
            }

            totals[currencyId] = updated;
        }

        return totals;
    }
}
