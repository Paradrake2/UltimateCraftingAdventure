using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllyGenerationConfig", menuName = "Scriptable Objects/Ally/Generation Config")]
public class AllyGenerationConfig : ScriptableObject
{
    [Header("Archetypes")]
    [SerializeField] private List<AllyArchetype> possibleArchetypes = new List<AllyArchetype>();

    [Header("Names")]
    [Tooltip("If empty, generated allies will be named 'Ally'.")]
    [SerializeField] private List<string> namePool = new List<string>();

    [Header("Stat Rolls (Additive)")]
    [Tooltip("Added on top of the archetype's BaseStats.")]
    [SerializeField] private List<StatRoll> additiveStatRolls = new List<StatRoll>();

    public IReadOnlyList<AllyArchetype> PossibleArchetypes => possibleArchetypes;
    public IReadOnlyList<string> NamePool => namePool;
    public IReadOnlyList<StatRoll> AdditiveStatRolls => additiveStatRolls;

    [Serializable]
    public class StatRoll
    {
        [SerializeField] private Stat stat;
        [SerializeField] private float min;
        [SerializeField] private float max;

        public Stat Stat => stat;
        public float Min => min;
        public float Max => max;

        public float RollValue()
        {
            float low = min;
            float high = max;
            if (low > high) (low, high) = (high, low);
            return UnityEngine.Random.Range(low, high);
        }
    }
}
