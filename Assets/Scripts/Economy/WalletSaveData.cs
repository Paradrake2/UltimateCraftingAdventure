using System;
using System.Collections.Generic;

[Serializable]
public class WalletSaveData
{
    public List<BalanceEntry> balances = new List<BalanceEntry>();

    [Serializable]
    public class BalanceEntry
    {
        public string currencyId;
        public long amount;
    }
}
