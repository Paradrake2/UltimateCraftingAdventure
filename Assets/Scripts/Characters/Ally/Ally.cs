using UnityEngine;

[CreateAssetMenu(fileName = "Ally", menuName = "Scriptable Objects/Ally")]
public class Ally : ScriptableObject
{
    [SerializeField] private string allyName;
    [SerializeField] private Sprite icon;
    [SerializeField] private StatCollection stats = new StatCollection();
    [SerializeField] private AllyArchetype archetype;
    [SerializeField] private AllyEquipmentInventory equipmentInventory = new AllyEquipmentInventory();
    public string AllyName => allyName;
    public Sprite Icon => icon;
    public StatCollection Stats => stats;
    public AllyArchetype Archetype => archetype;
    public AllyEquipmentInventory EquipmentInventory => equipmentInventory;

    public bool TryEquip(Equipment equipment, out string failureReason)
    {
        failureReason = null;

        if (equipmentInventory == null)
        {
            failureReason = "Cannot equip: no equipment inventory.";
            return false;
        }

        if (archetype != null && !archetype.CanEquip(this, equipmentInventory, equipment, out failureReason))
        {
            return false;
        }

        return equipmentInventory.TryEquip(equipment, out failureReason);
    }

    public bool TryUnequip(Equipment equipment, out string failureReason)
    {
        failureReason = null;

        if (equipmentInventory == null)
        {
            failureReason = "Cannot unequip: no equipment inventory.";
            return false;
        }

        if (archetype != null && !archetype.CanUnequip(this, equipmentInventory, equipment, out failureReason))
        {
            return false;
        }

        if (!equipmentInventory.TryUnequip(equipment))
        {
            failureReason = "Cannot unequip: item is not equipped.";
            return false;
        }

        return true;
    }

    public bool ValidateLoadout(out string failureReason)
    {
        failureReason = null;
        if (equipmentInventory == null) return true;
        if (archetype == null) return true;
        return archetype.ValidateLoadout(this, equipmentInventory, out failureReason);
    }
    public void RecalculateStats()
    {
        // placeholder
    }
}
