using System.Collections.Generic;
using UnityEngine;

public interface IEnchantmentEffect
{
    void ApplyEffect(Equipment equipment);
}
[System.Serializable]
public class EquipmentEnchantmentHolder
{
    [SerializeField] private Enchantment enchantment;
    public Enchantment Enchantment => enchantment;
    [SerializeField] private bool beenUsed = false;
    public bool BeenUsed => beenUsed;
    public EquipmentEnchantmentHolder(Enchantment enchantment)
    {
        this.enchantment = enchantment;
    }
    public void MarkAsUsed()    {
        beenUsed = true;
    }
}
public enum EnchantmentRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[System.Serializable]
public class EnchantmentCost
{
    [SerializeField] private List<ItemQuantity> requiredItems = new List<ItemQuantity>();
    public IReadOnlyList<ItemQuantity> RequiredItems => requiredItems;
    [SerializeField] private List<CurrencyCost> requiredCurrencies = new List<CurrencyCost>();
    public IReadOnlyList<CurrencyCost> RequiredCurrencies => requiredCurrencies;
    public bool CanAfford()
    {
        
        return true;
    }
}

[System.Serializable]
[CreateAssetMenu(fileName = "Enchantment", menuName = "Enchanting/Enchantment")]
public class Enchantment : ScriptableObject
{
    [SerializeField] private string enchantmentName;
    [SerializeField] private Sprite icon;
    [SerializeField] private IEnchantmentEffect effect;
    [SerializeField] private EnchantmentRarity rarity;
    [SerializeField] private EnchantmentCost cost;
    public IEnchantmentEffect Effect => effect;
    public string EnchantmentName => enchantmentName;
    public Sprite Icon => icon;
    public EnchantmentRarity Rarity => rarity;
    public EnchantmentCost Cost => cost;
    public Enchantment(string name, Sprite icon, IEnchantmentEffect effect, EnchantmentRarity rarity)
    {
        enchantmentName = name;
        this.icon = icon;
        this.effect = effect;
        this.rarity = rarity;
    }
    public void Apply(Equipment equipment)
    {
        if (equipment == null) return;
        
        effect.ApplyEffect(equipment);
    }
    public void SetEffect(IEnchantmentEffect newEffect)
    {
        effect = newEffect;
    }

}
