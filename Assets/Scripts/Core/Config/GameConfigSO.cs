using UnityEngine;

namespace ProjectManatite.Core
{
    /// <summary>
    /// Centralized magic-number registry. Drag a single asset of this type into the
    /// systems that need it (or load via Resources) instead of scattering hardcoded
    /// values across MonoBehaviours.
    ///
    /// Create one asset via <c>Assets &gt; Create &gt; Manatite &gt; Game Config</c>.
    /// </summary>
    [CreateAssetMenu(menuName = "Manatite/Game Config", fileName = "GameConfig")]
    public class GameConfigSO : ScriptableObject
    {
        [Header("Chips")]
        [Tooltip("Auto-destroy a dropped chip after this many seconds if not collected.")]
        public float ChipDropTimeout = 30f;

        [Tooltip("Hover-highlight delay between selecting and being able to re-select.")]
        public float ChipReselectDelay = 0.1f;

        [Tooltip("Time the chip stays selected after a click before auto-clearing.")]
        public float ChipSelectionLifetime = 1f;

        [Header("Damage Text")]
        [Tooltip("Total fade-out time of floating damage numbers (seconds).")]
        public float DamageTextFadeDuration = 0.6f;

        [Tooltip("Vertical drift speed of damage text (units/sec).")]
        public float DamageTextDriftSpeed = 0.6f;

        [Header("Projectiles")]
        [Tooltip("Default max travel distance before a projectile self-despawns.")]
        public float ProjectileDefaultMaxDistance = 10f;

        [Header("Settings")]
        [Tooltip("Seconds before an unconfirmed resolution change auto-reverts.")]
        public float ResolutionRevertDuration = 15f;

        [Header("Save System")]
        [Tooltip("Save file name placed under Application.persistentDataPath.")]
        public string SaveFileName = "save.json";
    }
}
