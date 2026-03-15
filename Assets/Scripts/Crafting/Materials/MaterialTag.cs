using UnityEngine;

[CreateAssetMenu(fileName = "MaterialTag", menuName = "Scriptable Objects/MaterialTag")]
public class MaterialTag : ScriptableObject
{
    [SerializeField] private string tagName;
    public string TagName => tagName;
}
