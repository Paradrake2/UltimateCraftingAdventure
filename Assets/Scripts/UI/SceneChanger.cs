using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private string baseScene = "Combat";

    private string _pendingOverlay;

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
    // If this was triggered by OpenOverlay, also unload any other overlays now
    // that the new scene is confirmed live.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode != LoadSceneMode.Additive) return;

        foreach (var root in scene.GetRootGameObjects())
        {
            var es = root.GetComponentInChildren<UnityEngine.EventSystems.EventSystem>();
            if (es != null
                && UnityEngine.EventSystems.EventSystem.current != null
                && UnityEngine.EventSystems.EventSystem.current != es
                && UnityEngine.EventSystems.EventSystem.current.gameObject.scene.name == baseScene)
                Destroy(es.gameObject);

            var al = root.GetComponentInChildren<AudioListener>();
            if (al != null) al.enabled = false;
        }

        if (_pendingOverlay == scene.name)
        {
            _pendingOverlay = null;
            for (int i = SceneManager.sceneCount - 1; i >= 0; i--)
            {
                Scene s = SceneManager.GetSceneAt(i);
                if (s.name != baseScene && s.name != scene.name && s.isLoaded)
                    SceneManager.UnloadSceneAsync(s);
            }
        }
    }

    // Opens the named scene additively, then unloads all other overlays once it is loaded.
    public void OpenOverlay(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName).isLoaded) return;

        _pendingOverlay = sceneName;
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

    // Returns to the combat base scene. Loads it the first time; after that just closes overlays.
    public void LoadCombatScene()
    {
        if (!SceneManager.GetSceneByName(baseScene).isLoaded)
        {
            SceneManager.LoadScene(baseScene);
            Debug.Log("Loading base scene additively");
            CloseAllOverlays();
        }
        else
        {
            Debug.Log("Base scene already loaded, just closing overlays");
            CloseAllOverlays();
        }
    }
}
