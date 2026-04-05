using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "EnchantmentDatabase", menuName = "Scriptable Objects/EnchantmentDatabase")]
public class EnchantmentDatabase : ScriptableObject
{
    [SerializeField] private List<Enchantment> enchantments = new List<Enchantment>(); // all enchantments
    public IReadOnlyList<Enchantment> Enchantments => enchantments;
    [SerializeField] private List<Enchantment> commonEnchantments = new List<Enchantment>();
    public IReadOnlyList<Enchantment> CommonEnchantments => commonEnchantments;
    [SerializeField] private List<Enchantment> uncommonEnchantments = new List<Enchantment>();
    public IReadOnlyList<Enchantment> UncommonEnchantments => uncommonEnchantments;
    [SerializeField] private List<Enchantment> rareEnchantments = new List<Enchantment>();
    public IReadOnlyList<Enchantment> RareEnchantments => rareEnchantments;
    [SerializeField] private List<Enchantment> epicEnchantments = new List<Enchantment>();
    public IReadOnlyList<Enchantment> EpicEnchantments => epicEnchantments;
    [SerializeField] private List<Enchantment> legendaryEnchantments = new List<Enchantment>();
    public IReadOnlyList<Enchantment> LegendaryEnchantments => legendaryEnchantments;

    private static EnchantmentDatabase instance;
    public static EnchantmentDatabase Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<EnchantmentDatabase>("EnchantmentDatabase");
                if (instance == null)
                {
                    Debug.LogError("EnchantmentDatabase asset not found in Resources folder.");
                }
            }
            return instance;
        }
    }
    public Enchantment GetRandomEnchantment(EnchantmentRarity rarity)
    {
        List<Enchantment> pool = rarity switch
        {
            EnchantmentRarity.Common => commonEnchantments,
            EnchantmentRarity.Uncommon => uncommonEnchantments,
            EnchantmentRarity.Rare => rareEnchantments,
            EnchantmentRarity.Epic => epicEnchantments,
            EnchantmentRarity.Legendary => legendaryEnchantments,
            _ => enchantments
        };
        if (pool.Count == 0)
        {
            Debug.LogWarning($"No enchantments of rarity {rarity} available in the database.");
            return null;
        }
        int randomIndex = UnityEngine.Random.Range(0, pool.Count);
        return pool[randomIndex];
    }
}
