using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private string baseScene = "Combat";

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // After any additive scene loads, destroy its EventSystem and disable its
    // AudioListener so only the base scene's copies remain active.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode != LoadSceneMode.Additive) return;

        foreach (var root in scene.GetRootGameObjects())
        {
            var es = root.GetComponentInChildren<UnityEngine.EventSystems.EventSystem>();
            if (es != null) Destroy(es.gameObject);

            var al = root.GetComponentInChildren<AudioListener>();
            if (al != null) al.enabled = false;
        }
    }

    // Closes all open overlays, then opens the named scene additively.
    public void OpenOverlay(string sceneName)
    {
        CloseAllOverlays();
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    // For close/back buttons — unloads a specific overlay.
    public void CloseOverlay(string sceneName)
    {
        if (!SceneManager.GetSceneByName(sceneName).isLoaded) return;
        SceneManager.UnloadSceneAsync(sceneName);
    }

    public void CloseAllOverlays()
    {
        for (int i = SceneManager.sceneCount - 1; i >= 0; i--)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name != baseScene && scene.isLoaded)
                SceneManager.UnloadSceneAsync(scene);
        }
    }

    // Full replace — only used for the initial transition into the Combat scene.
    public void LoadCombatScene()
    {
        SceneManager.LoadScene(baseScene);
        FindAnyObjectByType<Combat>()?.SceneLoaded();
    }
}
