using UnityEngine;

[System.Serializable]
public class BaseMaterialHolder
{
    [SerializeField] private BaseMaterials baseMaterial;
    [SerializeField] private int quantity;
    public BaseMaterials BaseMaterial => baseMaterial;
    public int Quantity => quantity;
}
