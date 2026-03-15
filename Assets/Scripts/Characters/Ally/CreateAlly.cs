using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CreateAlly
{
    [Header("Mode")]
    [SerializeField] private bool randomize;

    [Header("Explicit")]
    [SerializeField] private string allyName;
    [SerializeField] private AllyArchetype archetype;

    [Tooltip("If set, overrides the archetype icon.")]
    [SerializeField] private Sprite iconOverride;

    [Tooltip("If set and non-empty, overrides the archetype BaseStats.")]
    [SerializeField] private StatCollection statsOverride = new StatCollection();

    [Tooltip("Optional equipment to attempt to equip on creation.")]
    [SerializeField] private List<Equipment> startingEquipment = new List<Equipment>();

    [Header("Random")]
    [SerializeField] private AllyGenerationConfig randomConfig;

    [Header("(Legacy/Optional)")]
    [SerializeField] private EquipmentInventory equipmentInventory;

    public string AllyName => allyName;
    public AllyArchetype Archetype => archetype;
    public EquipmentInventory EquipmentInventory => equipmentInventory;

    public bool Randomize => randomize;
    public Sprite IconOverride => iconOverride;
    public StatCollection StatsOverride => statsOverride;
    public IReadOnlyList<Equipment> StartingEquipment => startingEquipment;
    public AllyGenerationConfig RandomConfig => randomConfig;

    public bool TryCreate(out Ally ally, out string failureReason)
    {
        if (randomize)
        {
            return AllyFactory.TryCreateRandom(randomConfig, out ally, out failureReason);
        }

        return AllyFactory.TryCreateExplicit(this, out ally, out failureReason);
    }

}
