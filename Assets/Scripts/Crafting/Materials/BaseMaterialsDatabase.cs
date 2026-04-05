using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseMaterialsDatabase", menuName = "Scriptable Objects/BaseMaterialsDatabase")]
public class BaseMaterialsDatabase : ScriptableObject
{
    [SerializeField] private List<BaseMaterial> baseMaterials = new List<BaseMaterial>();
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
