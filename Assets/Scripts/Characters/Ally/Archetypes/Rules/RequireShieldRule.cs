using UnityEngine;

[CreateAssetMenu(fileName = "RequireShieldRule", menuName = "Scriptable Objects/Ally/Rules/Require Shield")]
public class RequireShieldRule : AllyRestrictionRule
{
    [SerializeField] private bool blockUnequipShield = true;

    public override bool ValidateLoadout(Ally ally, AllyEquipmentInventory inventory, out string failureReason)
    {
        failureReason = null;
        if (inventory == null) return true;

        bool hasShield = (inventory.HandSlot1 != null && inventory.HandSlot1.EquipmentType == EquipmentType.Shield)
                        || (inventory.HandSlot2 != null && inventory.HandSlot2.EquipmentType == EquipmentType.Shield);

        if (!hasShield)
        {
            failureReason = "Invalid loadout: this ally must have a shield equipped.";
            return false;
        }

        return true;
    }

    public override bool CanUnequip(Ally ally, AllyEquipmentInventory inventory, Equipment equipment, out string failureReason)
    {
        failureReason = null;

        if (!blockUnequipShield) return true;

        if (equipment != null && equipment.EquipmentType == EquipmentType.Shield)
        {
            failureReason = "Cannot unequip: this ally must keep a shield equipped.";
            return false;
        }

        return true;
    }
}
