using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Handles serializing runtime game state to JSON on disk and restoring it on
/// load.  Call SaveSystem.Save() / SaveSystem.Load() from GameSaveManager or
/// any other appropriate point in your game.
/// </summary>
public static class SaveSystem
{
    private const string SaveFileName = "save.json";
    private static string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    // ─── Public API ───────────────────────────────────────────────────────────

    public static void Save()
    {
        var data = new SaveData();

        // Collect every piece of equipment that needs to persist:
        // both the unequipped pool and everything currently on an ally.
        var seenIds = new HashSet<string>();

        var equipInv = EquipmentInventory.Instance;
        if (equipInv != null)
        {
            foreach (var e in equipInv.OwnedEquipment)
            {
                if (e == null) continue;
                data.equipmentInventory.Add(EquipmentToSaveData(e));
                if (!string.IsNullOrEmpty(e.ID)) seenIds.Add(e.ID);
            }
        }

        // Also capture any equipment equipped on allies but not in the pool.
        var allyManager = AllyManager.Instance;
        if (allyManager != null)
        {
            foreach (var ally in allyManager.GetAllies())
            {
                if (ally?.EquipmentInventory == null) continue;
                foreach (var e in ally.EquipmentInventory.GetAllEquipped())
                {
                    if (e == null || string.IsNullOrEmpty(e.ID) || seenIds.Contains(e.ID)) continue;
                    data.equipmentInventory.Add(EquipmentToSaveData(e));
                    seenIds.Add(e.ID);
                }
            }

            foreach (var ally in allyManager.GetAllies())
                if (ally != null) data.allies.Add(AllyToSaveData(ally));
        }

        var itemInv = ItemInventory.Instance;
        if (itemInv != null)
        {
            foreach (var iq in itemInv.OwnedItems)
                if (iq?.Item != null)
                    data.itemInventory.Add(new ItemQuantitySaveData { itemName = iq.Item.name, quantity = iq.Quantity });
        }

        File.WriteAllText(SavePath, JsonUtility.ToJson(data, prettyPrint: true));
        Debug.Log($"[SaveSystem] Saved to {SavePath}");
    }

    public static bool Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("[SaveSystem] No save file found, using default starting state.");
            return false;
        }

        var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
        if (data == null) return false;

        // Equipment must be loaded first so ally slots can reference it by ID.
        var equipmentById = new Dictionary<string, Equipment>();

        // Collect all IDs that are equipped on an ally so we don't also add
        // them to the unequipped inventory pool — that would cause duplicates
        // when the player unequips an item.
        var equippedOnAllyIds = new HashSet<string>();
        if (data.allies != null)
        {
            foreach (var ad in data.allies)
            {
                var s = ad?.equipmentInventory;
                if (s == null) continue;
                AddIfNotEmpty(equippedOnAllyIds, s.helmetId);
                AddIfNotEmpty(equippedOnAllyIds, s.chestplateId);
                AddIfNotEmpty(equippedOnAllyIds, s.leggingsId);
                AddIfNotEmpty(equippedOnAllyIds, s.bootsId);
                AddIfNotEmpty(equippedOnAllyIds, s.glovesId);
                AddIfNotEmpty(equippedOnAllyIds, s.bracersId);
                AddIfNotEmpty(equippedOnAllyIds, s.handSlot1Id);
                AddIfNotEmpty(equippedOnAllyIds, s.handSlot2Id);
                AddIfNotEmpty(equippedOnAllyIds, s.accessorySlot1Id);
                AddIfNotEmpty(equippedOnAllyIds, s.accessorySlot2Id);
            }
        }

        var equipInv = EquipmentInventory.Instance;
        if (equipInv != null)
        {
            equipInv.Clear();
            foreach (var ed in data.equipmentInventory)
            {
                var e = EquipmentFromSaveData(ed);
                if (e == null) continue;
                if (!string.IsNullOrEmpty(ed.id))
                    equipmentById[ed.id] = e;
                // Only add to the unequipped pool if no ally is wearing it.
                if (!equippedOnAllyIds.Contains(ed.id))
                    equipInv.AddEquipment(e);
            }
        }

        var itemInv = ItemInventory.Instance;
        if (itemInv != null)
        {
            itemInv.Clear();
            foreach (var id in data.itemInventory)
            {
                var item = GameAssetRegistry.Instance?.FindItem(id.itemName);
                if (item != null)
                    itemInv.AddItem(item, id.quantity);
                else
                    Debug.LogWarning($"[SaveSystem] Item not found in registry: '{id.itemName}'");
            }
        }

        var allyManager = AllyManager.Instance;
        if (allyManager != null)
        {
            allyManager.ClearAllies();
            foreach (var ad in data.allies)
            {
                var ally = AllyFromSaveData(ad, equipmentById);
                if (ally != null) allyManager.AddAlly(ally);
            }
        }

        Debug.Log("[SaveSystem] Load complete.");
        return true;
    }

    // ─── Equipment ────────────────────────────────────────────────────────────

    private static EquipmentSaveData EquipmentToSaveData(Equipment e)
    {
        var data = new EquipmentSaveData
        {
            id                 = e.ID,
            equipmentName      = e.EquipmentName,
            equipmentType      = (int)e.EquipmentType,
            weaponDamageType   = (int)e.WeaponDamageType,
            rarity             = (int)e.Rarity,
            level              = e.Level,
            reinforcementLevel = e.ReinforcementLevel,
            generationModifier = (int)e.GenerationModifier,
            singleTagName      = e.Tag != null ? e.Tag.name : null,
            iconName           = e.Icon != null ? e.Icon.name : null,
        };

        if (e.BaseStats != null)
            foreach (var sv in e.BaseStats.Stats)
                if (sv?.Stat != null)
                    data.baseStats.Add(new StatValueSaveData { statName = sv.Stat.StatName, value = sv.Value });

        if (e.Stats != null)
            foreach (var sv in e.Stats.Stats)
                if (sv?.Stat != null)
                    data.stats.Add(new StatValueSaveData { statName = sv.Stat.StatName, value = sv.Value });

        if (e.Tags != null)
            foreach (var tag in e.Tags)
                if (tag != null) data.tagNames.Add(tag.name);

        if (e.Enchantments != null)
            foreach (var h in e.Enchantments)
                if (h?.Enchantment != null)
                    data.enchantments.Add(new EnchantmentSaveData { enchantmentName = h.Enchantment.name, beenUsed = h.BeenUsed });

        if (e.Augments != null)
            foreach (var h in e.Augments)
                if (h?.Augment != null)
                    data.augmentNames.Add(h.Augment.name);

        return data;
    }

    private static Equipment EquipmentFromSaveData(EquipmentSaveData data)
    {
        var registry = GameAssetRegistry.Instance;
        if (registry == null)
        {
            Debug.LogError("[SaveSystem] GameAssetRegistry not found in Resources.");
            return null;
        }

        var baseStats = BuildStatCollection(data.baseStats, registry);
        var stats     = BuildStatCollection(data.stats, registry);
        var icon      = string.IsNullOrEmpty(data.iconName) ? null : registry.FindSprite(data.iconName);

        var equipment = ScriptableObject.CreateInstance<Equipment>();
        equipment.RestoreFromSave(
            data.id,
            data.equipmentName,
            (EquipmentType)data.equipmentType,
            (DamageType)data.weaponDamageType,
            (EquipmentRarity)data.rarity,
            data.level,
            data.reinforcementLevel,
            (EquipmentGenerationModifier)data.generationModifier,
            baseStats,
            stats,
            icon
        );

        if (data.tagNames != null)
            foreach (var tagName in data.tagNames)
            {
                var tag = registry.FindTag(tagName);
                if (tag != null) equipment.AddTag(tag);
                else Debug.LogWarning($"[SaveSystem] EquipmentTag not found: '{tagName}'");
            }

        if (!string.IsNullOrEmpty(data.singleTagName))
            equipment.SetTag(registry.FindTag(data.singleTagName));

        if (data.enchantments != null)
            foreach (var ed in data.enchantments)
            {
                var enchantment = registry.FindEnchantment(ed.enchantmentName);
                if (enchantment == null)
                {
                    Debug.LogWarning($"[SaveSystem] Enchantment not found: '{ed.enchantmentName}'");
                    continue;
                }
                var holder = new EquipmentEnchantmentHolder(enchantment);
                if (ed.beenUsed) holder.MarkAsUsed();
                equipment.AddEnchantmentHolder(holder);
            }

        if (data.augmentNames != null)
            foreach (var augmentName in data.augmentNames)
            {
                var augment = registry.FindAugment(augmentName);
                if (augment != null) equipment.AddAugment(augment);
                else Debug.LogWarning($"[SaveSystem] Augment not found: '{augmentName}'");
            }

        return equipment;
    }

    // ─── Ally ─────────────────────────────────────────────────────────────────

    private static AllySaveData AllyToSaveData(Ally ally)
    {
        var data = new AllySaveData
        {
            allyName      = ally.AllyName,
            archetypeName = ally.Archetype != null ? ally.Archetype.name : null,
            iconName      = ally.Icon != null ? ally.Icon.name : null,
            xp            = ally.XP,
            level         = ally.Level,
            xpToNextLevel = ally.XPToNextLevel,
        };

        if (ally.Stats != null)
            foreach (var sv in ally.Stats.Stats)
                if (sv?.Stat != null)
                    data.stats.Add(new StatValueSaveData { statName = sv.Stat.StatName, value = sv.Value });

        if (ally.CombatStats?.SkillSlots != null)
            foreach (var slot in ally.CombatStats.SkillSlots)
                if (slot?.Skill != null)
                    data.skillSlots.Add(new SkillSlotSaveData { slotType = (int)slot.SlotType, skillName = slot.Skill.name });

        var inv = ally.EquipmentInventory;
        if (inv != null)
            data.equipmentInventory = new AllyEquipmentSaveData
            {
                helmetId         = inv.Helmet?.ID,
                chestplateId     = inv.Chestplate?.ID,
                leggingsId       = inv.Leggings?.ID,
                bootsId          = inv.Boots?.ID,
                glovesId         = inv.Gloves?.ID,
                bracersId        = inv.Bracers?.ID,
                handSlot1Id      = inv.HandSlot1?.ID,
                handSlot2Id      = inv.HandSlot2?.ID,
                accessorySlot1Id = inv.AccessorySlot1?.ID,
                accessorySlot2Id = inv.AccessorySlot2?.ID,
            };

        var runeInv = ally.RuneInventory;
        if (runeInv != null)
        {
            var slots = runeInv.Slots;
            data.runeInventory = new AllyRuneSaveData
            {
                slot0 = RuneSlotToSaveData(slots[0]),
                slot1 = RuneSlotToSaveData(slots[1]),
                slot2 = RuneSlotToSaveData(slots[2]),
            };
        }

        return data;
    }

    private static Ally AllyFromSaveData(AllySaveData data, Dictionary<string, Equipment> equipmentById)
    {
        var registry  = GameAssetRegistry.Instance;
        var archetype = registry?.FindArchetype(data.archetypeName);
        var icon      = string.IsNullOrEmpty(data.iconName) ? null : registry?.FindSprite(data.iconName);
        var stats     = BuildStatCollection(data.stats, registry);

        var ally = Ally.CreateRuntime(data.allyName, archetype, icon, stats);
        ally.RestoreProgress(data.xp, data.level, data.xpToNextLevel, stats);

        if (data.skillSlots != null && data.skillSlots.Count > 0)
        {
            foreach (var ss in data.skillSlots)
            {
                if (ss == null || string.IsNullOrEmpty(ss.skillName)) continue;
                var skill = registry?.FindSkill(ss.skillName);
                if (skill == null)
                {
                    Debug.LogWarning($"[SaveSystem] Skill not found in registry: '{ss.skillName}'. Add it to GameAssetRegistry.");
                    continue;
                }
                ally.CombatStats?.RestoreSkillInSlot((SkillSlotType)ss.slotType, skill);
            }
        }

        if (data.equipmentInventory != null)
        {
            var s = data.equipmentInventory;
            ally.EquipmentInventory.RestoreSlots(
                Lookup(s.helmetId, equipmentById),
                Lookup(s.chestplateId, equipmentById),
                Lookup(s.leggingsId, equipmentById),
                Lookup(s.bootsId, equipmentById),
                Lookup(s.glovesId, equipmentById),
                Lookup(s.bracersId, equipmentById),
                Lookup(s.handSlot1Id, equipmentById),
                Lookup(s.handSlot2Id, equipmentById),
                Lookup(s.accessorySlot1Id, equipmentById),
                Lookup(s.accessorySlot2Id, equipmentById)
            );
        }

        if (data.runeInventory != null)
        {
            var runeRegistry = GameAssetRegistry.Instance;
            var rs = data.runeInventory;
            var slotData = new[] { rs.slot0, rs.slot1, rs.slot2 };
            var unlocked = new bool[AllyRuneInventory.SlotCount];
            var runes    = new Rune[AllyRuneInventory.SlotCount];
            for (int i = 0; i < AllyRuneInventory.SlotCount; i++)
            {
                if (slotData[i] == null) continue;
                unlocked[i] = slotData[i].isUnlocked;
                if (!string.IsNullOrEmpty(slotData[i].runeName))
                {
                    runes[i] = runeRegistry?.FindRune(slotData[i].runeName);
                    if (runes[i] == null)
                        Debug.LogWarning($"[SaveSystem] Rune not found in registry: '{slotData[i].runeName}'. Add it to GameAssetRegistry.");
                }
            }
            ally.RuneInventory.RestoreSlots(unlocked, runes);
        }

        ally.RecalculateStats();
        return ally;
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private static StatCollection BuildStatCollection(List<StatValueSaveData> savedStats, GameAssetRegistry registry)
    {
        var collection = new StatCollection();
        if (savedStats == null || registry == null) return collection;

        foreach (var sd in savedStats)
        {
            var stat = registry.FindStat(sd.statName);
            if (stat != null)
                collection.SetStat(stat, sd.value);
            else
                Debug.LogWarning($"[SaveSystem] Stat not found in registry: '{sd.statName}'");
        }

        return collection;
    }

    private static Equipment Lookup(string id, Dictionary<string, Equipment> map)
    {
        if (string.IsNullOrEmpty(id)) return null;
        map.TryGetValue(id, out var e);
        return e;
    }

    private static void AddIfNotEmpty(HashSet<string> set, string value)
    {
        if (!string.IsNullOrEmpty(value)) set.Add(value);
    }

    private static AllyRuneSlotSaveData RuneSlotToSaveData(AllyRuneInventory.RuneSlot slot)
    {
        if (slot == null) return new AllyRuneSlotSaveData();
        return new AllyRuneSlotSaveData
        {
            isUnlocked = slot.IsUnlocked,
            runeName   = slot.Rune != null ? slot.Rune.name : string.Empty,
        };
    }
}
