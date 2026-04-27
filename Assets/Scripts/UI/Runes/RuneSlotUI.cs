using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Base UI class for a single rune slot. Attach to a slot GameObject and wire
/// up the serialized fields in the Inspector.
///
/// Override <see cref="OnRuneEquipped"/>, <see cref="OnRuneUnequipped"/>, and
/// <see cref="OnSlotLockStateChanged"/> in subclasses to add game-specific
/// behaviour (animations, SFX, tooltips, etc.).
/// </summary>
public class RuneSlotUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image runeIcon;
    [SerializeField] private TextMeshProUGUI runeNameLabel;
    [SerializeField] private GameObject lockedOverlay;   // shown when slot is locked
    [SerializeField] private GameObject emptyIndicator;  // shown when unlocked but empty
    [SerializeField] private Sprite emptySlotSprite;     // fallback icon when no rune is equipped

    // ─── Runtime state ────────────────────────────────────────────────────────

    private Ally _ally;
    private int _slotIndex;

    public Ally Ally => _ally;
    public int SlotIndex => _slotIndex;

    /// <summary>Convenience accessor for the slot this UI represents.</summary>
    protected AllyRuneInventory.RuneSlot Slot =>
        _ally?.RuneInventory?.Slots != null && _slotIndex >= 0 && _slotIndex < AllyRuneInventory.SlotCount
            ? _ally.RuneInventory.Slots[_slotIndex]
            : null;

    // ─── Initialisation ───────────────────────────────────────────────────────

    /// <summary>
    /// Binds this UI to a specific rune slot on an ally. Call this from your
    /// parent panel when it sets up the ally view.
    /// </summary>
    public void Initialize(Ally ally, int slotIndex)
    {
        _ally       = ally;
        _slotIndex  = slotIndex;
        Refresh();
    }

    // ─── Public API ───────────────────────────────────────────────────────────

    /// <summary>
    /// Equips <paramref name="rune"/> into this slot and refreshes the display.
    /// </summary>
    public bool TryEquipRune(Rune rune, out string failureReason)
    {
        if (_ally == null)
        {
            failureReason = "No ally assigned to this rune slot UI.";
            return false;
        }

        bool success = _ally.TryEquipRuneInSlot(_slotIndex, rune, out failureReason);
        if (success)
        {
            Refresh();
            OnRuneEquipped(rune);
        }
        return success;
    }

    /// <summary>
    /// Removes the rune currently in this slot and refreshes the display.
    /// </summary>
    public bool TryUnequipRune(out string failureReason)
    {
        if (_ally == null)
        {
            failureReason = "No ally assigned to this rune slot UI.";
            return false;
        }

        var slot = Slot;
        if (slot?.Rune == null)
        {
            failureReason = "Slot is already empty.";
            return false;
        }

        var rune = slot.Rune;
        bool success = _ally.TryUnequipRune(rune, out failureReason);
        if (success)
        {
            Refresh();
            OnRuneUnequipped(rune);
        }
        return success;
    }

    /// <summary>
    /// Unlocks this specific slot and refreshes the display.
    /// </summary>
    public bool TryUnlockSlot()
    {
        if (_ally?.RuneInventory == null) return false;

        bool success = _ally.RuneInventory.UnlockSlot(_slotIndex);
        if (success)
        {
            Refresh();
            OnSlotLockStateChanged(isLocked: false);
        }
        return success;
    }

    /// <summary>
    /// Rebuilds all visual elements to match the current slot state.
    /// Call this after any external change to the ally's rune inventory.
    /// </summary>
    public void Refresh()
    {
        var slot = Slot;
        bool isLocked  = slot == null || !slot.IsUnlocked;
        bool hasRune   = slot?.Rune != null;

        // Locked overlay
        if (lockedOverlay != null)
            lockedOverlay.SetActive(isLocked);

        // Empty indicator (unlocked but nothing in the slot)
        if (emptyIndicator != null)
            emptyIndicator.SetActive(!isLocked && !hasRune);

        // Icon
        if (runeIcon != null)
        {
            runeIcon.enabled = hasRune || emptySlotSprite != null;
            runeIcon.sprite  = hasRune ? slot.Rune.Icon : emptySlotSprite;
        }

        // Name label
        if (runeNameLabel != null)
            runeNameLabel.text = hasRune ? slot.Rune.RuneName : string.Empty;
    }

    // ─── Overridable hooks ────────────────────────────────────────────────────

    /// <summary>Called after a rune has been successfully equipped into this slot.</summary>
    protected virtual void OnRuneEquipped(Rune rune) { }

    /// <summary>Called after a rune has been successfully removed from this slot.</summary>
    protected virtual void OnRuneUnequipped(Rune rune) { }

    /// <summary>Called after this slot's lock state changes.</summary>
    protected virtual void OnSlotLockStateChanged(bool isLocked) { }
}
