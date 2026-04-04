using UnityEngine;
using System.Collections.Generic;
public class BasicMissionManager : MonoBehaviour
{
    [SerializeField] private List<CombatMap> combatMaps = new List<CombatMap>();
    [SerializeField] private List<GameObject> missionButtons = new List<GameObject>();

    public void BasicMissionLoad()
    {
        foreach (GameObject buttonObj in missionButtons)
        {
            BasicMissionButton button = buttonObj.GetComponent<BasicMissionButton>();
            if (button != null)
            {
                CombatMap map = button.GetCombatMap();
                if (map != null && (map.CanUnlock() || !map.IsLocked))
                {
                    button.Unlock();
                }
            }
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
