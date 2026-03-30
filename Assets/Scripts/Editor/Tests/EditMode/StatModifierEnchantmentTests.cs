using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class StatModifierEnchantmentTests
{
    private readonly List<Object> createdObjects = new List<Object>();
    
    [TearDown]
    public void TearDown()
    {
        for (int index = createdObjects.Count - 1; index >= 0; index--)
        {
            if (createdObjects[index] != null)
            {
                Object.DestroyImmediate(createdObjects[index]);
            }
        }

        createdObjects.Clear();
    }

    [Test]
    public void ApplyEffect_Multiplies_Targeted_Stat()
    {
        Stat targetedStat = CreateStat();
        Equipment equipment = CreateEquipment((targetedStat, 10f));
        StatModifierEnchantment enchantment = CreateEnchantment(1.5f, targetedStat);

        enchantment.ApplyEffect(equipment);

        Assert.That(equipment.GetStatValue(targetedStat), Is.EqualTo(15f));
    }
    [Test]
    public void Equipment_Apply_Enchant_Test()
    {
        Stat targetedStat = CreateStat();
        Equipment equipment = CreateEquipment((targetedStat, 10f));
        StatModifierEnchantment enchantment = CreateEnchantment(1.5f, targetedStat);
        Enchantment enchantmentWrapper = ScriptableObject.CreateInstance<Enchantment>();
        enchantmentWrapper.SetEffect(enchantment);
        equipment.AddEnchantment(enchantmentWrapper);
        equipment.ApplyEnchantments();
        

        Assert.That(equipment.GetStatValue(targetedStat), Is.EqualTo(15f));
    }
    [Test]
    public void ApplyEffect_Does_Not_Change_NonTargeted_Stat()
    {
        Stat targetedStat = CreateStat();
        Stat untouchedStat = CreateStat();
        Equipment equipment = CreateEquipment((targetedStat, 10f), (untouchedStat, 25f));
        StatModifierEnchantment enchantment = CreateEnchantment(2f, targetedStat);

        enchantment.ApplyEffect(equipment);

        Assert.That(equipment.GetStatValue(targetedStat), Is.EqualTo(20f));
        Assert.That(equipment.GetStatValue(untouchedStat), Is.EqualTo(25f));
    }

    [Test]
    public void ApplyEffect_Multiplies_All_Targeted_Stats()
    {
        Stat firstStat = CreateStat();
        Stat secondStat = CreateStat();
        Stat thirdStat = CreateStat();
        Equipment equipment = CreateEquipment((firstStat, 10f), (secondStat, 20f), (thirdStat, 30f));
        StatModifierEnchantment enchantment = CreateEnchantment(2f, firstStat, secondStat);

        enchantment.ApplyEffect(equipment);

        Assert.That(equipment.GetStatValue(firstStat), Is.EqualTo(20f));
        Assert.That(equipment.GetStatValue(secondStat), Is.EqualTo(40f));
        Assert.That(equipment.GetStatValue(thirdStat), Is.EqualTo(30f));
    }

    [Test]
    public void AddStat_Does_Not_Apply_Duplicate_Targets_More_Than_Once()
    {
        Stat targetedStat = CreateStat();
        Equipment equipment = CreateEquipment((targetedStat, 12f));
        StatModifierEnchantment enchantment = CreateEnchantment(2f);

        enchantment.AddStat(targetedStat);
        enchantment.AddStat(targetedStat);

        enchantment.ApplyEffect(equipment);

        Assert.That(equipment.GetStatValue(targetedStat), Is.EqualTo(24f));
    }

    [Test]
    public void ApplyEffect_With_Empty_Targets_Does_Not_Change_Equipment()
    {
        Stat existingStat = CreateStat();
        Equipment equipment = CreateEquipment((existingStat, 8f));
        StatModifierEnchantment enchantment = CreateEnchantment(3f);

        enchantment.ApplyEffect(equipment);

        Assert.That(equipment.GetStatValue(existingStat), Is.EqualTo(8f));
    }

    [Test]
    public void ApplyStatModifier_Multiplies_Single_Stat_Value()
    {
        Stat stat = CreateStat();
        Equipment equipment = CreateEquipment((stat, 7f));

        equipment.ApplyStatModifier(stat, 2.5f);

        Assert.That(equipment.GetStatValue(stat), Is.EqualTo(17.5f));
    }

    private StatModifierEnchantment CreateEnchantment(float modifierAmount = 1f, params Stat[] stats)
    {
        StatModifierEnchantment enchantment = ScriptableObject.CreateInstance<StatModifierEnchantment>();
        createdObjects.Add(enchantment);
        enchantment.SetStats(new List<Stat>(stats));
        enchantment.SetModifierAmount(modifierAmount);
        return enchantment;
    }

    private Equipment CreateEquipment(params (Stat stat, float value)[] statEntries)
    {
        Equipment equipment = ScriptableObject.CreateInstance<Equipment>();
        createdObjects.Add(equipment);

        StatCollection statCollection = new StatCollection();
        foreach (var (stat, value) in statEntries)
        {
            statCollection.SetStat(stat, value);
        }

        equipment.SetStats(statCollection);
        return equipment;
    }

    private Stat CreateStat()
    {
        Stat stat = ScriptableObject.CreateInstance<Stat>();
        createdObjects.Add(stat);
        return stat;
    }
    
}