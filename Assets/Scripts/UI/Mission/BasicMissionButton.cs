using UnityEngine;

public class BasicMissionButton : MonoBehaviour
{
    [SerializeField] private CombatMap combatMap;
    [SerializeField] private SceneChanger sceneChanger;
    [SerializeField] private bool isLocked;
    public void OnClick()
    {
        if (combatMap == null || isLocked) return;

        MissionManager.SelectMap(combatMap);
        sceneChanger.LoadCombatScene();
    }
    public void Unlock()
    {
        isLocked = false;
    }
    public CombatMap GetCombatMap()
    {
        return combatMap;
    }
}
