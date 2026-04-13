using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public List<EquipmentSaveData> equipmentInventory = new List<EquipmentSaveData>();
    public List<ItemQuantitySaveData> itemInventory = new List<ItemQuantitySaveData>();
    public List<AllySaveData> allies = new List<AllySaveData>();
}

[Serializable]
public class StatValueSaveData
{
    public string statName;
    public float value;
}

[Serializable]
public class EquipmentSaveData
{
    public string id;
    public string equipmentName;
    public int equipmentType;       // cast to/from EquipmentType
    public int rarity;              // cast to/from EquipmentRarity
    public int level;
    public int reinforcementLevel;
    public int generationModifier;  // cast to/from EquipmentGenerationModifier
    public string iconName;
    public string singleTagName;
    public List<StatValueSaveData> baseStats = new List<StatValueSaveData>();
    public List<StatValueSaveData> stats = new List<StatValueSaveData>();
    public List<string> tagNames = new List<string>();
    public List<EnchantmentSaveData> enchantments = new List<EnchantmentSaveData>();
    public List<string> augmentNames = new List<string>();
}

[Serializable]
public class EnchantmentSaveData
{
    public string enchantmentName;
    public bool beenUsed;
}

[Serializable]
public class ItemQuantitySaveData
{
    public string itemName;
    public int quantity;
}

[Serializable]
public class AllySaveData
{
    public string allyName;
    public string archetypeName;
    public string iconName;
    public float xp;
    public int level;
    public float xpToNextLevel;
    public List<StatValueSaveData> stats = new List<StatValueSaveData>();
    public List<string> skillNames = new List<string>();
    public AllyEquipmentSaveData equipmentInventory = new AllyEquipmentSaveData();
}

[Serializable]
public class AllyEquipmentSaveData
{
    public string helmetId;
    public string chestplateId;
    public string leggingsId;
    public string bootsId;
    public string glovesId;
    public string bracersId;
    public string handSlot1Id;
    public string handSlot2Id;
    public string accessorySlot1Id;
    public string accessorySlot2Id;
}
