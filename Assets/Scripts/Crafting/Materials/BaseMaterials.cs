using UnityEngine;

// This is for generic base materials like wood, metal, not specific items. These do not have stats

[CreateAssetMenu(fileName = "BaseMaterials", menuName = "Crafting/BaseMaterials")]
public class BaseMaterials : ScriptableObject
{
    [SerializeField] private string materialName;
    [SerializeField] private Sprite materialIcon;
    public string MaterialName => materialName;
    public Sprite MaterialIcon => materialIcon;
}
