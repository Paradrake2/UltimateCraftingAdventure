using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite icon;
    [SerializeField] private StatCollection statCol = new StatCollection();
    public string ItemName => itemName;
    public Sprite Icon => icon;
    public StatCollection StatCol => statCol;
}
