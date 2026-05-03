using TMPro;
using UnityEngine;

/// <summary>
/// Floating chip-info popup. Spawned by <see cref="Chip.ShowTooltip"/> when the
/// cursor enters a chip; displays the chip's name and effect text.
///
/// Refactor notes: empty <c>Start</c> / <c>Update</c> removed. Title / Effect
/// text references are cached on first <see cref="UpdateInfo"/> instead of
/// being looked up via <c>transform.Find</c> every refresh.
/// </summary>
public class Tooltip : MonoBehaviour
{
    [Tooltip("Local-space offset applied to the popup when shown next to a chip.")]
    public Vector2 toolPlacement;

    [SerializeField] private TextMeshPro titleText;
    [SerializeField] private TextMeshPro effectText;

    /// <summary>Backwards-compatible alias for callers using the original casing.</summary>
    public void updateInfo(Chip chipObj) => UpdateInfo(chipObj);

    public void UpdateInfo(Chip chipObj)
    {
        if (chipObj == null || chipObj.chipSO == null) return;

        // Resolve children once and cache. Falling back to Find for prefabs that
        // don't have the inspector references set (preserves the original behaviour).
        if (titleText == null)
        {
            var title = transform.Find("Title");
            if (title != null) title.TryGetComponent(out titleText);
        }
        if (effectText == null)
        {
            var effect = transform.Find("Effect");
            if (effect != null) effect.TryGetComponent(out effectText);
        }

        if (titleText != null)  titleText.text  = chipObj.chipSO.chipName;
        if (effectText != null) effectText.text = chipObj.chipSO.effect;
    }
}
