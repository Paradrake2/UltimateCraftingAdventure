using UnityEngine;

public class ItemInventory : MonoBehaviour
{
    private static ItemInventory _instance;
    public static ItemInventory Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }
}
