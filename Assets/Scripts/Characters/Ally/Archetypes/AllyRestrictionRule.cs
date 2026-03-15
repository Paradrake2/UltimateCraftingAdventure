using UnityEngine;

public abstract class AllyRestrictionRule : ScriptableObject
{
    public virtual bool CanEquip(Ally ally, AllyEquipmentInventory inventory, Equipment equipment, out string failureReason)
    {
        failureReason = null;
        return true;
    }

    public virtual bool CanUnequip(Ally ally, AllyEquipmentInventory inventory, Equipment equipment, out string failureReason)
    {
        failureReason = null;
        return true;
    }

    public virtual bool ValidateLoadout(Ally ally, AllyEquipmentInventory inventory, out string failureReason)
    {
        failureReason = null;
        return true;
    }
}
