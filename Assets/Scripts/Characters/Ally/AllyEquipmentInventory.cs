using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AllyEquipmentInventory
{
	[Header("Armor")]
	[SerializeField] private Equipment helmet;
	[SerializeField] private Equipment chestplate;
	[SerializeField] private Equipment leggings;
	[SerializeField] private Equipment boots;
	[SerializeField] private Equipment gloves;
	[SerializeField] private Equipment bracers;

	[Header("Hands")]
	[SerializeField] private Equipment handSlot1;
	[SerializeField] private Equipment handSlot2;

	[Header("Accessories")]
	[SerializeField] private Equipment accessorySlot1;
	[SerializeField] private Equipment accessorySlot2;

	public Equipment Helmet => helmet;
	public Equipment Chestplate => chestplate;
	public Equipment Leggings => leggings;
	public Equipment Boots => boots;
	public Equipment Gloves => gloves;
	public Equipment Bracers => bracers;

	public Equipment HandSlot1 => handSlot1;
	public Equipment HandSlot2 => handSlot2;
	public Equipment AccessorySlot1 => accessorySlot1;
	public Equipment AccessorySlot2 => accessorySlot2;

	public IEnumerable<Equipment> GetAllEquipped()
	{
		if (helmet != null) yield return helmet;
		if (chestplate != null) yield return chestplate;
		if (leggings != null) yield return leggings;
		if (boots != null) yield return boots;
		if (gloves != null) yield return gloves;
		if (bracers != null) yield return bracers;
		if (handSlot1 != null) yield return handSlot1;
		if (handSlot2 != null) yield return handSlot2;
		if (accessorySlot1 != null) yield return accessorySlot1;
		if (accessorySlot2 != null) yield return accessorySlot2;
	}

	public bool IsEquipped(Equipment equipment)
	{
		if (equipment == null) return false;
		return helmet == equipment
			   || chestplate == equipment
			   || leggings == equipment
			   || boots == equipment
			   || gloves == equipment
			   || bracers == equipment
			   || handSlot1 == equipment
			   || handSlot2 == equipment
			   || accessorySlot1 == equipment
			   || accessorySlot2 == equipment;
	}

	public bool TryEquip(Equipment equipment, out string failureReason)
	{
		failureReason = null;

		if (equipment == null)
		{
			failureReason = "Cannot equip: equipment is null.";
			return false;
		}

		if (IsEquipped(equipment))
		{
			failureReason = "Cannot equip: already equipped.";
			return false;
		}

		switch (equipment.EquipmentType)
		{
			case EquipmentType.Helmet:
				if (helmet != null)
				{
					failureReason = "Cannot equip: helmet slot already filled.";
					return false;
				}
				helmet = equipment;
				return true;

			case EquipmentType.Chestplate:
				if (chestplate != null)
				{
					failureReason = "Cannot equip: chestplate slot already filled.";
					return false;
				}
				chestplate = equipment;
				return true;

			case EquipmentType.Leggings:
				if (leggings != null)
				{
					failureReason = "Cannot equip: leggings slot already filled.";
					return false;
				}
				leggings = equipment;
				return true;
			case EquipmentType.Bracers:
				if (bracers != null)
				{
					failureReason = "Cannot equip: bracers slot already filled.";
					return false;
				}
				bracers = equipment;
				return true;
			case EquipmentType.Gloves:
				if (gloves != null)				{
					failureReason = "Cannot equip: gloves slot already filled.";
					return false;
				}
				gloves = equipment;
				return true;
			case EquipmentType.Boots:
				if (boots != null)
				{
					failureReason = "Cannot equip: boots slot already filled.";
					return false;
				}
				boots = equipment;
				return true;

			case EquipmentType.Accessory:
				if (accessorySlot1 == null)
				{
					accessorySlot1 = equipment;
					return true;
				}
				if (accessorySlot2 == null)
				{
					accessorySlot2 = equipment;
					return true;
				}
				failureReason = "Cannot equip: both accessory slots are already filled.";
				return false;

			case EquipmentType.Weapon:
			case EquipmentType.Shield:
				return TryEquipToHands(equipment, out failureReason);

			default:
				failureReason = $"Cannot equip: unsupported equipment type '{equipment.EquipmentType}'.";
				return false;
		}
	}

	public bool TryUnequip(Equipment equipment)
	{
		if (equipment == null) return false;

		if (helmet == equipment) { helmet = null; return true; }
		if (chestplate == equipment) { chestplate = null; return true; }
		if (leggings == equipment) { leggings = null; return true; }
		if (boots == equipment) { boots = null; return true; }
		if (gloves == equipment) { gloves = null; return true; }
		if (bracers == equipment) { bracers = null; return true; }
		if (leggings == equipment) { leggings = null; return true; }
		if (boots == equipment) { boots = null; return true; }
		if (handSlot1 == equipment) { handSlot1 = null; return true; }
		if (handSlot2 == equipment) { handSlot2 = null; return true; }
		if (accessorySlot1 == equipment) { accessorySlot1 = null; return true; }
		if (accessorySlot2 == equipment) { accessorySlot2 = null; return true; }

		return false;
	}

	public bool TryUnequip(EquipmentType equipmentType, out Equipment unequipped)
	{
		unequipped = null;

		switch (equipmentType)
		{
			case EquipmentType.Helmet:
				unequipped = helmet;
				helmet = null;
				return unequipped != null;

			case EquipmentType.Chestplate:
				unequipped = chestplate;
				chestplate = null;
				return unequipped != null;

			case EquipmentType.Leggings:
				unequipped = leggings;
				leggings = null;
				return unequipped != null;

			case EquipmentType.Boots:
				unequipped = boots;
				boots = null;
				return unequipped != null;
			case EquipmentType.Gloves:
				unequipped = gloves;
				gloves = null;
				return unequipped != null;
			case EquipmentType.Bracers:
				unequipped = bracers;
				bracers = null;
				return unequipped != null;

			case EquipmentType.Accessory:
				if (accessorySlot2 != null) { unequipped = accessorySlot2; accessorySlot2 = null; return true; }
				if (accessorySlot1 != null) { unequipped = accessorySlot1; accessorySlot1 = null; return true; }
				return false;

			case EquipmentType.Weapon:
			case EquipmentType.Shield:
				// Prefer removing from slot2 first to feel like "offhand".
				if (handSlot2 != null && handSlot2.EquipmentType == equipmentType) { unequipped = handSlot2; handSlot2 = null; return true; }
				if (handSlot1 != null && handSlot1.EquipmentType == equipmentType) { unequipped = handSlot1; handSlot1 = null; return true; }
				return false;

			default:
				return false;
		}
	}

	private bool TryEquipToHands(Equipment equipment, out string failureReason)
	{
		failureReason = null;

		bool slot1Empty = handSlot1 == null;
		bool slot2Empty = handSlot2 == null;

		if (!slot1Empty && !slot2Empty)
		{
			failureReason = "Cannot equip: both hand slots are already filled.";
			return false;
		}

		// If the player is trying to equip a shield, enforce that the resulting state is valid:
		// either (Weapon + Shield) or (Weapon + Weapon). Shield alone is not allowed.
		if (equipment.EquipmentType == EquipmentType.Shield)
		{
			if (HasShieldEquipped())
			{
				failureReason = "Cannot equip: only 1 shield can be equipped.";
				return false;
			}

			if (!HasWeaponEquipped())
			{
				failureReason = "Cannot equip: a shield requires at least 1 weapon equipped.";
				return false;
			}

			// Put shield in the first empty slot.
			if (slot1Empty) handSlot1 = equipment;
			else handSlot2 = equipment;

			return true;
		}

		// Equipping a weapon is always allowed if there is an empty hand slot,
		// unless we somehow already have 2 weapons (should be caught by full slots).
		if (equipment.EquipmentType == EquipmentType.Weapon)
		{
			if (slot1Empty) handSlot1 = equipment;
			else handSlot2 = equipment;
			return true;
		}

		failureReason = "Cannot equip: only weapons and shields can be equipped in hands.";
		return false;
	}

	private bool HasWeaponEquipped()
	{
		return (handSlot1 != null && handSlot1.EquipmentType == EquipmentType.Weapon)
			   || (handSlot2 != null && handSlot2.EquipmentType == EquipmentType.Weapon);
	}

	private bool HasShieldEquipped()
	{
		return (handSlot1 != null && handSlot1.EquipmentType == EquipmentType.Shield)
			   || (handSlot2 != null && handSlot2.EquipmentType == EquipmentType.Shield);
	}
}
