using UnityEngine;

[System.Serializable]
public class CurrencyCost
{
    [SerializeField] private Currency currency;
    [SerializeField] private float amount;

    public Currency Currency => currency;
    public float Amount => amount;
}


[CreateAssetMenu(fileName = "Currency", menuName = "Scriptable Objects/Economy/Currency")]
public class Currency : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField] private Sprite icon;

    public string Id => id;
    public string DisplayName => displayName;
    public Sprite Icon => icon;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            id = System.Guid.NewGuid().ToString("N");
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            displayName = name;
        }
    }
#endif
}
