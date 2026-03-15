using System;
using System.IO;
using UnityEngine;

public static class WalletPersistence
{
    private const string FileName = "wallet.json";

    public static string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, FileName);
    }

    public static WalletSaveData LoadOrCreate()
    {
        string path = GetSavePath();

        try
        {
            if (!File.Exists(path))
            {
                return new WalletSaveData();
            }

            string json = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(json))
            {
                return new WalletSaveData();
            }

            var data = JsonUtility.FromJson<WalletSaveData>(json);
            return data ?? new WalletSaveData();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Failed to load wallet data. Creating new wallet. Path: {path}. Error: {ex.Message}");
            return new WalletSaveData();
        }
    }

    public static bool TrySave(WalletSaveData data, out string failureReason)
    {
        failureReason = null;
        if (data == null)
        {
            failureReason = "Cannot save wallet: data is null.";
            return false;
        }

        string path = GetSavePath();

        try
        {
            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonUtility.ToJson(data, prettyPrint: true);
            File.WriteAllText(path, json);
            return true;
        }
        catch (Exception ex)
        {
            failureReason = $"Failed to save wallet. Path: {path}. Error: {ex.Message}";
            return false;
        }
    }
}
