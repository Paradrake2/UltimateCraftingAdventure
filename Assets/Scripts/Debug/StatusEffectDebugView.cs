using UnityEngine;

public class StatusEffectDebugView : MonoBehaviour
{
    [SerializeField] private Combat combat;

    private void OnGUI()
    {
        if (combat == null) return;
        GUILayout.BeginArea(new Rect(10, 10, 300, 600));
        GUI.color = Color.red;
        foreach (var enemy in combat.Enemies)
        {
            if (enemy?.StatusEffects == null) continue;
            GUILayout.Label($"[{enemy.EnemyName}]");
            foreach (var fx in enemy.StatusEffects.ActiveEffects)
                GUILayout.Label($"  {fx.Definition.EffectName} — {fx.RemainingDuration:F1}s");
        }
        GUILayout.EndArea();
    }
}
