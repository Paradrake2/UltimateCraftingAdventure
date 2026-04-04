using System;
using UnityEngine;

public static class MissionManager
{
    public static CombatMap SelectedMap { get; private set; }
    public static event Action<CombatMap> OnMapSelected;

    public static void SelectMap(CombatMap map)
    {
        SelectedMap = map;
        OnMapSelected?.Invoke(map);
    }
}
