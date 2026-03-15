using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentTag", menuName = "Scriptable Objects/Equipment/Tag")]
public class EquipmentTag : ScriptableObject
{
    [SerializeField] private string tagName;
    public string TagName => tagName;
}
