using UnityEngine;

/// <summary>
/// Place this MonoBehaviour in your first/persistent scene.
/// It auto-loads on Start and auto-saves when the application quits.
/// Call SaveGame() / LoadGame() from UI buttons or other code as needed.
/// </summary>
public class GameSaveManager : MonoBehaviour
{
    private static GameSaveManager _instance;
    public static GameSaveManager Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SaveSystem.Load();
    }

    private void OnApplicationQuit()
    {
        SaveSystem.Save();
    }

    public void SaveGame() => SaveSystem.Save();
    public void LoadGame() => SaveSystem.Load();
}
