using UnityEngine;

[CreateAssetMenu(fileName = "DisallowShieldsRule", menuName = "Scriptable Objects/Ally/Rules/Disallow Shields")]
public class DisallowShieldsRule : AllyRestrictionRule
{
    public override bool CanEquip(Ally ally, AllyEquipmentInventory inventory, Equipment equipment, out string failureReason)
    {
        failureReason = null;

        if (equipment != null && equipment.EquipmentType == EquipmentType.Shield)
        {
            failureReason = "Cannot equip: this ally cannot use shields.";
            return false;
        }

        return true;
    }
}
