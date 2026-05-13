using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ally", menuName = "Scriptable Objects/AllyCustom")]
public class Ally : ScriptableObject, IStatusEffectTarget, IHealable
{
    [SerializeField] private string allyName;
    [SerializeField] private Sprite icon;
    [SerializeField] private StatCollection stats = new StatCollection();
    [SerializeField] private AllyArchetype archetype;
    [SerializeField] private AllyEquipmentInventory equipmentInventory = new AllyEquipmentInventory();
    [SerializeField] private AllyRuneInventory runeInventory = new AllyRuneInventory();
    [SerializeField] private AllyCombat combatStats = new AllyCombat();
    [SerializeField] private float xp = 0f;
    [SerializeField] private int level = 1;
    [SerializeField] private float xpToNextLevel = 100f;
    [SerializeField] private float xpGrowthRate = 1.1f;
    public string AllyName => allyName;
    public Sprite Icon => icon;
    public StatCollection Stats => stats;
    public AllyArchetype Archetype => archetype;
    public AllyEquipmentInventory EquipmentInventory => equipmentInventory;
    public AllyRuneInventory RuneInventory => runeInventory;
    public AllyCombat CombatStats => combatStats;
    public float XP => xp;
    public int Level => level;
    public float XPToNextLevel => xpToNextLevel;
    public float XPGrowthRate => xpGrowthRate;

    // IStatusEffectTarget
    public StatusEffectManager StatusEffects => combatStats?.StatusEffects;

    // IHealable
    public double CurrentHealth => combatStats?.CurrentHealth ?? 0d;
    public double MaxHealth => combatStats?.GetMaxHealth() ?? 0d;
    public void Heal(double amount) => combatStats?.Heal(amount);

    public void Initialize(string name, Sprite newIcon, StatCollection newStats, AllyArchetype newArchetype)
    {
        allyName = name;
        icon = newIcon;
        stats = newStats ?? new StatCollection();
        archetype = newArchetype;

        equipmentInventory ??= new AllyEquipmentInventory();
        runeInventory ??= new AllyRuneInventory();
        combatStats ??= new AllyCombat();
        combatStats.Initialize(stats, this);

        level = 1;
        xp = 0f;
        xpToNextLevel = 100f;
        xpGrowthRate = 1.1f;

        // Apply archetype default skills if none are set yet (safety net for any creation path)
        if (archetype != null && combatStats.SkillSlots.Count == 0 && archetype.DefaultSkills != null && archetype.DefaultSkills.Count > 0)
            combatStats.SetSkills(archetype.DefaultSkills);
    }

    public static Ally CreateRuntime(string name, AllyArchetype archetype, Sprite icon = null, StatCollection stats = null)
    {
        var ally = CreateInstance<Ally>();

        string resolvedName = string.IsNullOrWhiteSpace(name) ? "Ally" : name;
        Sprite resolvedIcon = icon != null ? icon : archetype != null ? archetype.Icon : null;
        StatCollection resolvedStats = stats ?? (archetype != null ? archetype.BaseStats.Clone() : new StatCollection());

        ally.Initialize(resolvedName, resolvedIcon, resolvedStats, archetype);
        return ally;
    }
    public Ally(string name, Sprite icon, StatCollection stats, AllyArchetype archetype)
    {
        allyName = name;
        this.icon = icon;
        this.stats = stats;
        this.archetype = archetype;
    }
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

        bool success = equipmentInventory.TryEquip(equipment, out failureReason);
        if (success) RecalculateStats();
        return success;
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

    // ─── Rune methods ─────────────────────────────────────────────────────────

    /// <summary>
    /// Equips a rune into the first available unlocked slot. Fails if no slot is
    /// free, a duplicate type is already equipped, or all slots are locked.
    /// </summary>
    public bool TryEquipRune(Rune rune, out string failureReason)
    {
        runeInventory ??= new AllyRuneInventory();
        return runeInventory.TryEquip(rune, out failureReason);
    }

    /// <summary>
    /// Equips a rune into a specific slot by index.
    /// </summary>
    public bool TryEquipRuneInSlot(int slotIndex, Rune rune, out string failureReason)
    {
        runeInventory ??= new AllyRuneInventory();
        return runeInventory.TryEquipInSlot(slotIndex, rune, out failureReason);
    }

    /// <summary>
    /// Removes a rune from whichever slot it occupies.
    /// </summary>
    public bool TryUnequipRune(Rune rune, out string failureReason)
    {
        if (runeInventory == null)
        {
            failureReason = "Cannot unequip rune: no rune inventory.";
            return false;
        }
        return runeInventory.TryUnequip(rune, out failureReason);
    }

    /// <summary>
    /// Unlocks the next locked rune slot. Returns true if a slot was unlocked.
    /// </summary>
    public bool UnlockNextRuneSlot(out int slotIndex)
    {
        runeInventory ??= new AllyRuneInventory();
        return runeInventory.UnlockNextSlot(out slotIndex);
    }

    public bool ValidateLoadout(out string failureReason)
    {
        failureReason = null;
        if (equipmentInventory == null) return true;
        if (archetype == null) return true;
        return archetype.ValidateLoadout(this, equipmentInventory, out failureReason);
    }
    /// <summary>
    /// Replaces the skill in a slot, enforcing archetype tag restrictions.
    /// Pass <c>null</c> as <paramref name="newSkill"/> to clear the slot.
    /// </summary>
    public bool TryReplaceSkill(int slotIndex, AllySkill newSkill, out string failureReason)
    {
        failureReason = null;

        if (combatStats == null)
        {
            failureReason = "Cannot replace skill: ally has no combat stats.";
            return false;
        }

        if (newSkill != null && archetype != null && !archetype.CanEquipSkill(newSkill, out failureReason))
        {
            return false;
        }

        if (!combatStats.SetSkillInSlot(slotIndex, newSkill))
        {
            failureReason = $"Cannot replace skill: slot index {slotIndex} is out of range (ally has {combatStats.SkillSlots?.Count ?? 0} skill slots).";
            return false;
        }

        return true;
    }

    public void RecalculateStats()
    {
        StatCollection merged = stats.Clone();

        if (equipmentInventory != null)
        {
            foreach (Equipment equipment in equipmentInventory.GetAllEquipped())
            {
                if (equipment == null) continue;
                foreach (var sv in equipment.BaseStats.Stats)
                    if (sv?.Stat != null) merged.AddStatValue(sv.Stat, sv.Value);
                foreach (var sv in equipment.Stats.Stats)
                    if (sv?.Stat != null) merged.AddStatValue(sv.Stat, sv.Value);
            }
        }

        combatStats?.Initialize(merged);
    }
    public void AddXP(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        xp += amount;

        while (xp >= xpToNextLevel)
        {
            xp -= xpToNextLevel;
            level++;
            xpToNextLevel = Mathf.Max(1f, xpToNextLevel * Mathf.Max(0.01f, xpGrowthRate));
        }
        RecalculateStats();
    }

    /// <summary>
    /// Restores runtime progression fields from save data.
    /// Called by SaveSystem — do not call from other code.
    /// </summary>
    public void RestoreProgress(float savedXp, int savedLevel, float savedXpToNextLevel, StatCollection savedStats)
    {
        xp            = savedXp;
        level         = Mathf.Max(1, savedLevel);
        xpToNextLevel = Mathf.Max(1f, savedXpToNextLevel);
        if (savedStats != null)
        {
            stats = savedStats;
            combatStats?.Initialize(stats);
        }
    }

    // ─── Combat lifecycle ───────────────────────────────────────────────────────

    [System.NonSerialized] private bool _isInCombat;
    public bool IsInCombat => _isInCombat;

    /// <summary>
    /// Called by the combat system when this ally enters a session.
    /// Locks rune modifications, resets per-combat rune state, and fires
    /// the utility rune's <see cref="UtilityRune.OnCombatStart"/> hook.
    /// </summary>
    public void EnterCombat()
    {
        _isInCombat = true;
        if (runeInventory != null)
        {
            runeInventory.ResetRunesForCombat(this);
            runeInventory.LockForCombat();
            runeInventory.TriggerUtilityOnCombatStart(this);
        }
    }

    /// <summary>Called by the combat system when the combat session ends. Unlocks rune modifications.</summary>
    public void ExitCombat()
    {
        if (!_isInCombat) return;
        _isInCombat = false;
        runeInventory?.UnlockForCombat();
    }

    /// <summary>
    /// Entry point for all incoming enemy damage during combat.
    /// Runs equipped defense runes before applying damage, then applies
    /// any retaliation damage to the attacker.
    /// </summary>
    public void ReceiveAttack(List<DamageInstance> instances, Enemy attacker)
    {
        if (combatStats == null || !combatStats.IsAlive) return;

        var ctx = new DefenseRuneContext(new List<DamageInstance>(instances), this, attacker);

        runeInventory?.TriggerDefenseRunes(ctx);

        if (!ctx.IsCancelled && ctx.DamageInstances.Count > 0)
            combatStats.TakeDamage(ctx.DamageInstances);

        if (ctx.RetaliationInstances != null && ctx.RetaliationInstances.Count > 0
            && attacker?.CombatStats != null && attacker.CombatStats.IsAlive)
            attacker.CombatStats.TakeDamage(ctx.RetaliationInstances);
        // Thorns: deal flat physical damage back to the attacker, unaffected by damage multipliers.
        float thorns = stats.GetStatValue("Thorns");
        if (thorns > 0f && attacker?.CombatStats != null && attacker.CombatStats.IsAlive)
        {
            float thornsMultiplier = stats.GetStatValue("ThornsMultiplier");
            double thornsDamage = thorns * thornsMultiplier;
            attacker.CombatStats.TakeDamage(new List<DamageInstance> { new DamageInstance(DamageType.Physical, thornsDamage) });
        }

        // If the ally just died, give the utility rune a chance to intervene (e.g. resurrection).
        if (!combatStats.IsAlive)
        {
            runeInventory?.TriggerUtilityOnAllyDied(this);
            if (!combatStats.IsAlive) // still dead after any potential revival
                GameEventManager.FireAllyDied(new AllyDiedArgs(this, attacker));
        }
    }
}
