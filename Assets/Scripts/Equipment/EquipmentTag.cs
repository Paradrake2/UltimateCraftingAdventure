using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentTag", menuName = "Scriptable Objects/EquipmentTag")]
public class EquipmentTag : ScriptableObject
{
    [SerializeField] private string tagName;
    public string TagName => tagName;
}
