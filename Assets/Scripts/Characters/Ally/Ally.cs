using UnityEngine;

[CreateAssetMenu(fileName = "Ally", menuName = "Scriptable Objects/AllyCustom")]
public class Ally : ScriptableObject
{
    [SerializeField] private string allyName;
    [SerializeField] private Sprite icon;
    [SerializeField] private StatCollection stats = new StatCollection();
    [SerializeField] private AllyArchetype archetype;
    [SerializeField] private AllyEquipmentInventory equipmentInventory = new AllyEquipmentInventory();
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
    public AllyCombat CombatStats => combatStats;
    public float XP => xp;
    public int Level => level;
    public float XPToNextLevel => xpToNextLevel;
    public float XPGrowthRate => xpGrowthRate;

    public void Initialize(string name, Sprite newIcon, StatCollection newStats, AllyArchetype newArchetype)
    {
        allyName = name;
        icon = newIcon;
        stats = newStats ?? new StatCollection();
        archetype = newArchetype;

        equipmentInventory ??= new AllyEquipmentInventory();
        combatStats ??= new AllyCombat();
    combatStats.Initialize(stats);
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
        combatStats?.Initialize(stats);
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
            xpToNextLevel *= xpGrowthRate;
            RecalculateStats();
        }
    }
}
