using UnityEngine;

[CreateAssetMenu(fileName = "DisallowDualWieldWeaponsRule", menuName = "Scriptable Objects/Ally/Rules/Disallow Dual Wield")]
public class DisallowDualWieldWeaponsRule : AllyRestrictionRule
{
    public override bool CanEquip(Ally ally, AllyEquipmentInventory inventory, Equipment equipment, out string failureReason)
    {
        failureReason = null;

        if (inventory == null || equipment == null) return true;
        if (equipment.EquipmentType != EquipmentType.Weapon) return true;

        bool hasWeaponInHand1 = inventory.HandSlot1 != null && inventory.HandSlot1.EquipmentType == EquipmentType.Weapon;
        bool hasWeaponInHand2 = inventory.HandSlot2 != null && inventory.HandSlot2.EquipmentType == EquipmentType.Weapon;

        // If a weapon is already equipped, don't allow a second weapon.
        if (hasWeaponInHand1 || hasWeaponInHand2)
        {
            failureReason = "Cannot equip: this ally cannot dual wield weapons.";
            return false;
        }

        return true;
    }
}
