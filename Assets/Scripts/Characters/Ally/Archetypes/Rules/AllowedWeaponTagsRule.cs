using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllowedWeaponTagsRule", menuName = "Scriptable Objects/Ally/Rules/Allowed Weapon Tags")]
public class AllowedWeaponTagsRule : AllyRestrictionRule
{
    [SerializeField] private List<EquipmentTag> allowedWeaponTags = new List<EquipmentTag>();

    public override bool CanEquip(Ally ally, AllyEquipmentInventory inventory, Equipment equipment, out string failureReason)
    {
        failureReason = null;

        if (equipment == null) return true;
        if (equipment.EquipmentType != EquipmentType.Weapon) return true;

        // Empty list means "no restriction".
        if (allowedWeaponTags == null || allowedWeaponTags.Count == 0) return true;

        if (equipment.HasAnyTag(allowedWeaponTags)) return true;

        failureReason = "Cannot equip: this ally cannot use that weapon.";
        return false;
    }
}
