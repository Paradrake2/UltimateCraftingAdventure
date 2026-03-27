using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseMaterialsDatabase", menuName = "Scriptable Objects/BaseMaterialsDatabase")]
public class BaseMaterialsDatabase : ScriptableObject
{
    [SerializeField] private List<BaseMaterial> baseMaterials = new List<BaseMaterial>();
    private Dictionary<BaseMaterial, int> baseMaterialDictionary;
    private void OnEnable()
    {
        baseMaterialDictionary = new Dictionary<BaseMaterial, int>();
        foreach (var material in baseMaterials)
        {
            baseMaterialDictionary[material] = material.Quantity;
        }
    }
    public int GetBaseMaterialQuantity(BaseMaterial material)
    {
        if (baseMaterialDictionary.TryGetValue(material, out var quantity))
        {
            return quantity;
        }
        else
        {
            Debug.LogWarning($"Base material '{material}' not found in database.");
            return 0;
        }
    }
    public IReadOnlyList<BaseMaterial> BaseMaterials => baseMaterials.AsReadOnly();
    private static BaseMaterialsDatabase instance;
    public static BaseMaterialsDatabase Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<BaseMaterialsDatabase>("BaseMaterialsDatabase");
                if (instance == null)
                {
                    Debug.LogError("BaseMaterialsDatabase asset not found in Resources folder.");
                }
            }
            return instance;
        }
    }

}
