using UnityEngine;

[CreateAssetMenu(fileName = "LifestealAugment", menuName = "Scriptable Objects/LifestealAugment")]
public class LifestealAugment : ScriptableObject, IAugmentEffect
{
    [SerializeField] private float lifestealPercentage;
    public float LifestealPercentage => lifestealPercentage;
    public void ApplyEffect(Equipment equipment, Ally ally)
    {
        double damage = equipment.Stats.GetStatValue("Damage");
        double lifestealAmount = damage * lifestealPercentage;
    }
}
