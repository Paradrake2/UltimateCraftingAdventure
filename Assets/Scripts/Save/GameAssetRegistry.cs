using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A ScriptableObject registry that maps design-time assets to names so the
/// save system can look them up by name at load time.
/// Create one asset via Assets > Create > Save > Game Asset Registry, place it
/// in a Resources folder, and populate each list in the Inspector.
/// </summary>
[CreateAssetMenu(fileName = "GameAssetRegistry", menuName = "Save/Game Asset Registry")]
public class GameAssetRegistry : ScriptableObject
{
    [SerializeField] private List<Stat> stats = new List<Stat>();
    [SerializeField] private List<Item> items = new List<Item>();
    [SerializeField] private List<EquipmentTag> equipmentTags = new List<EquipmentTag>();
    [SerializeField] private List<Enchantment> enchantments = new List<Enchantment>();
    [SerializeField] private List<Augment> augments = new List<Augment>();
    [SerializeField] private List<AllyArchetype> archetypes = new List<AllyArchetype>();

    [System.NonSerialized] private Dictionary<string, Sprite> _spriteCache;

    private static GameAssetRegistry _instance;

    public static GameAssetRegistry Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<GameAssetRegistry>("GameAssetRegistry");
                if (_instance == null)
                    Debug.LogError("[GameAssetRegistry] Asset not found in Resources. Create one via Assets > Create > Save > Game Asset Registry and place it in a Resources folder.");
            }
            return _instance;
        }
    }

    public Stat FindStat(string statName) =>
        stats?.Find(s => s != null && s.StatName == statName);

    public Item FindItem(string itemName) =>
        items?.Find(i => i != null && i.name == itemName);

    public EquipmentTag FindTag(string tagName) =>
        equipmentTags?.Find(t => t != null && t.name == tagName);

    public Enchantment FindEnchantment(string enchantmentName) =>
        enchantments?.Find(e => e != null && e.name == enchantmentName);

    public Augment FindAugment(string augmentName) =>
        augments?.Find(a => a != null && a.name == augmentName);

    public AllyArchetype FindArchetype(string archetypeName) =>
        archetypes?.Find(a => a != null && a.name == archetypeName);

    public Sprite FindSprite(string spriteName)
    {
        if (string.IsNullOrEmpty(spriteName)) return null;

        if (_spriteCache == null)
        {
            _spriteCache = new Dictionary<string, Sprite>();
            foreach (var sprite in Resources.LoadAll<Sprite>(""))
                if (sprite != null && !_spriteCache.ContainsKey(sprite.name))
                    _spriteCache[sprite.name] = sprite;
        }

        _spriteCache.TryGetValue(spriteName, out var result);
        return result;
    }
}
