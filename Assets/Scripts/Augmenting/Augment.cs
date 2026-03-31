using UnityEngine;

// augments add special effects like lifesteal. Basically, non-stat modifiers
// applied to equipment, triggered when equipment is used
public interface IAugmentEffect
{
    void ApplyEffect(Equipment equipment, Ally ally);
}
[System.Serializable]
public class EquipmentAugmentHolder
{
    [SerializeField] private Augment augment;
    public bool BeenUsed { get; private set; } = false;
    public Augment Augment => augment;
    public EquipmentAugmentHolder(Augment augment)
    {
        this.augment = augment;
    }
}
[CreateAssetMenu(fileName = "Augment", menuName = "Augment/Augment")]
public class Augment : ScriptableObject
{
    [SerializeField] private string augmentName;
    [SerializeField] private Sprite icon;
    [SerializeField] private IAugmentEffect effect;
    public string AugmentName => augmentName;
    public Sprite Icon => icon;
    public IAugmentEffect Effect => effect;
    public Augment(string name, Sprite icon, IAugmentEffect effect)
    {
        augmentName = name;
        this.icon = icon;
        this.effect = effect;
    }
}
