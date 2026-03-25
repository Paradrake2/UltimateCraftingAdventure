using UnityEngine;

// These items are for crafting. They are used in recipes

[CreateAssetMenu(fileName = "Item", menuName = "Crafting/Item")]
public class Item : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite icon;
    [SerializeField] private StatCollection statCol = new StatCollection();
    public string ItemName => itemName;
    public Sprite Icon => icon;
    public StatCollection StatCol => statCol;
}
