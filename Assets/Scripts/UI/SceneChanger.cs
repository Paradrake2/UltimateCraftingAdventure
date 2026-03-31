using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    public void LoadCharacterMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("CharacterMenu");
    }
    public void LoadCombatScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Combat");
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
