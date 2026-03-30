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

[System.Serializable]
[CreateAssetMenu(fileName = "Enchantment", menuName = "Enchanting/Enchantment")]
public class Enchantment : ScriptableObject
{
    [SerializeField] private string enchantmentName;
    [SerializeField] private Sprite icon;
    [SerializeField] private IEnchantmentEffect effect;
    public IEnchantmentEffect Effect => effect;
    public string EnchantmentName => enchantmentName;
    public Sprite Icon => icon;
    public Enchantment(string name, Sprite icon, IEnchantmentEffect effect)
    {
        enchantmentName = name;
        this.icon = icon;
        this.effect = effect;
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
