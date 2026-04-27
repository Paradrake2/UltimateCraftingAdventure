using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds exactly three rune slots for an ally, with fixed types:
///   Slot 0 — Defense
///   Slot 1 — Defense
///   Slot 2 — Utility
///
/// Slots are locked by default and must be unlocked individually.
/// The entire inventory is locked for modifications during active combat.
/// No two runes of the same concrete C# class may occupy the inventory simultaneously.
/// </summary>
[Serializable]
public class AllyRuneInventory
{
    public const int SlotCount = 3;

    private static readonly RuneType[] SlotTypes =
    {
        RuneType.Defense,
        RuneType.Defense,
        RuneType.Utility,
    };

    /// <summary>Returns the <see cref="RuneType"/> required by the slot at <paramref name="slotIndex"/>.</summary>
    public static RuneType GetRequiredType(int slotIndex) =>
        (slotIndex >= 0 && slotIndex < SlotCount) ? SlotTypes[slotIndex] : RuneType.Defense;

    // ─── Slot ─────────────────────────────────────────────────────────────────

    [Serializable]
    public class RuneSlot
    {
        [SerializeField] private bool isUnlocked;
        [SerializeField] private Rune rune;

        public bool IsUnlocked => isUnlocked;
        public Rune Rune       => rune;

        public void Unlock() => isUnlocked = true;
        public void Lock()   { isUnlocked = false; rune = null; }

        internal void SetRune(Rune value) => rune = value;
    }

    [SerializeField] private RuneSlot[] slots = new RuneSlot[SlotCount];

    /// <summary>True during active combat — equip/unequip/unlock are blocked.</summary>
    [System.NonSerialized] private bool _isLockedForCombat;

    public AllyRuneInventory()
    {
        slots = new RuneSlot[SlotCount];
        for (int i = 0; i < SlotCount; i++)
            slots[i] = new RuneSlot();
    }

    // ─── Properties ───────────────────────────────────────────────────────────

    public IReadOnlyList<RuneSlot> Slots         => slots;
    public bool IsLockedForCombat                => _isLockedForCombat;

    // ─── Combat lock ──────────────────────────────────────────────────────────

    public void LockForCombat()   => _isLockedForCombat = true;
    public void UnlockForCombat() => _isLockedForCombat = false;

    // ─── Unlock slots ─────────────────────────────────────────────────────────

    /// <summary>Unlocks the next locked slot in order. Returns false if all slots are already unlocked or combat-locked.</summary>
    public bool UnlockNextSlot(out int unlockedIndex)
    {
        if (_isLockedForCombat) { unlockedIndex = -1; return false; }
        for (int i = 0; i < SlotCount; i++)
        {
            if (!slots[i].IsUnlocked)
            {
                slots[i].Unlock();
                unlockedIndex = i;
                return true;
            }
        }
        unlockedIndex = -1;
        return false;
    }

    /// <summary>Unlocks the slot at <paramref name="index"/>. Returns false if already unlocked, out of range, or combat-locked.</summary>
    public bool UnlockSlot(int index)
    {
        if (_isLockedForCombat) return false;
        if (index < 0 || index >= SlotCount) return false;
        if (slots[index].IsUnlocked) return false;
        slots[index].Unlock();
        return true;
    }

    // ─── Equip ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Equips <paramref name="rune"/> into the first available unlocked empty slot
    /// whose <see cref="RuneType"/> matches <c>rune.RuneType</c>.
    /// </summary>
    public bool TryEquip(Rune rune, out string failureReason)
    {
        if (!ValidateCanModify(rune, out failureReason)) return false;
        if (IsEquipped(rune))            { failureReason = $"'{rune.RuneName}' is already equipped."; return false; }
        if (HasRuneOfSameClass(rune.GetType())) { failureReason = $"A rune of class '{rune.GetType().Name}' is already equipped."; return false; }

        for (int i = 0; i < SlotCount; i++)
        {
            if (SlotTypes[i] == rune.RuneType && slots[i].IsUnlocked && slots[i].Rune == null)
            {
                slots[i].SetRune(rune);
                failureReason = null;
                return true;
            }
        }

        failureReason = $"No unlocked {rune.RuneType} rune slot is available.";
        return false;
    }

    /// <summary>
    /// Equips <paramref name="rune"/> into the specific slot at <paramref name="slotIndex"/>.
    /// The rune's <see cref="Rune.RuneType"/> must match the slot's required type.
    /// </summary>
    public bool TryEquipInSlot(int slotIndex, Rune rune, out string failureReason)
    {
        if (!ValidateCanModify(rune, out failureReason)) return false;
        if (slotIndex < 0 || slotIndex >= SlotCount)    { failureReason = $"Slot index {slotIndex} is out of range."; return false; }
        if (!slots[slotIndex].IsUnlocked)               { failureReason = $"Rune slot {slotIndex} is locked."; return false; }
        if (rune.RuneType != SlotTypes[slotIndex])      { failureReason = $"Slot {slotIndex} requires a {SlotTypes[slotIndex]} rune, but '{rune.RuneName}' is {rune.RuneType}."; return false; }
        if (IsEquipped(rune))                           { failureReason = $"'{rune.RuneName}' is already equipped."; return false; }
        if (HasRuneOfSameClass(rune.GetType(), excludeSlot: slotIndex)) { failureReason = $"A rune of class '{rune.GetType().Name}' is already equipped."; return false; }

        slots[slotIndex].SetRune(rune);
        failureReason = null;
        return true;
    }

    // ─── Unequip ──────────────────────────────────────────────────────────────

    /// <summary>Removes <paramref name="rune"/> from whichever slot it occupies.</summary>
    public bool TryUnequip(Rune rune, out string failureReason)
    {
        if (_isLockedForCombat) { failureReason = "Cannot modify runes during combat."; return false; }
        if (rune == null)       { failureReason = "Cannot unequip a null rune."; return false; }

        for (int i = 0; i < SlotCount; i++)
        {
            if (slots[i].Rune == rune)
            {
                slots[i].SetRune(null);
                failureReason = null;
                return true;
            }
        }

        failureReason = $"'{rune.RuneName}' is not equipped.";
        return false;
    }

    /// <summary>Removes the rune in the slot at <paramref name="slotIndex"/>.</summary>
    public bool TryUnequipSlot(int slotIndex, out string failureReason)
    {
        if (_isLockedForCombat)                        { failureReason = "Cannot modify runes during combat."; return false; }
        if (slotIndex < 0 || slotIndex >= SlotCount)   { failureReason = $"Slot index {slotIndex} is out of range."; return false; }
        if (slots[slotIndex].Rune == null)             { failureReason = $"Slot {slotIndex} is already empty."; return false; }

        slots[slotIndex].SetRune(null);
        failureReason = null;
        return true;
    }

    // ─── Queries ──────────────────────────────────────────────────────────────

    public bool IsEquipped(Rune rune)
    {
        if (rune == null) return false;
        foreach (var slot in slots)
            if (slot.Rune == rune) return true;
        return false;
    }

    public IEnumerable<Rune> GetAllEquipped()
    {
        foreach (var slot in slots)
            if (slot.Rune != null) yield return slot.Rune;
    }

    // ─── Combat triggers ──────────────────────────────────────────────────────

    /// <summary>Runs both Defense rune slots against <paramref name="ctx"/> in slot order (0 then 1).</summary>
    public void TriggerDefenseRunes(DefenseRuneContext ctx)
    {
        for (int i = 0; i < SlotCount; i++)
            if (SlotTypes[i] == RuneType.Defense && slots[i].Rune is DefenseRune dr)
                dr.ApplyEffect(ctx);
    }

    public void TriggerUtilityOnCombatStart(Ally ally)
    {
        if (GetUtilityRune() is UtilityRune ur) ur.OnCombatStart(ally);
    }

    public void TriggerUtilityOnAllyDied(Ally ally)
    {
        if (GetUtilityRune() is UtilityRune ur) ur.OnAllyDied(ally);
    }

    public void TriggerUtilityOnEnemyDied(Ally ally, Enemy enemy)
    {
        if (GetUtilityRune() is UtilityRune ur) ur.OnEnemyDied(ally, enemy);
    }

    public void TriggerUtilityOnWaveCleared(Ally ally)
    {
        if (GetUtilityRune() is UtilityRune ur) ur.OnWaveCleared(ally);
    }

    /// <summary>Resets per-combat state on all equipped runes. Called at the start of each combat session.</summary>
    public void ResetRunesForCombat(Ally ally)
    {
        for (int i = 0; i < SlotCount; i++)
        {
            if (SlotTypes[i] == RuneType.Defense && slots[i].Rune is DefenseRune dr)
                dr.ResetForCombat();
            else if (SlotTypes[i] == RuneType.Utility && slots[i].Rune is UtilityRune ur)
                ur.ResetForCombat(ally);
        }
    }

    // ─── Restore (save system) ────────────────────────────────────────────────

    /// <summary>Restores slot state directly from save data without validation.</summary>
    public void RestoreSlots(bool[] unlocked, Rune[] runes)
    {
        for (int i = 0; i < SlotCount; i++)
        {
            if (unlocked != null && i < unlocked.Length && unlocked[i])
                slots[i].Unlock();
            if (runes != null && i < runes.Length && runes[i] != null)
                slots[i].SetRune(runes[i]);
        }
    }

    // ─── Private helpers ──────────────────────────────────────────────────────

    private bool ValidateCanModify(Rune rune, out string failureReason)
    {
        if (_isLockedForCombat) { failureReason = "Cannot modify runes during combat."; return false; }
        if (rune == null)       { failureReason = "Cannot equip a null rune."; return false; }
        failureReason = null;
        return true;
    }

    private bool HasRuneOfSameClass(Type runeClass, int excludeSlot = -1)
    {
        for (int i = 0; i < SlotCount; i++)
        {
            if (i == excludeSlot) continue;
            if (slots[i].Rune != null && slots[i].Rune.GetType() == runeClass)
                return true;
        }
        return false;
    }

    private UtilityRune GetUtilityRune() => slots[2].Rune as UtilityRune;
}
