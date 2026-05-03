using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Animation orchestrator for the main-menu reveal: ramps fire-distortion
/// intensity on a list of <see cref="PseudoMovimentoFogo"/> instances and
/// scroll speed on a list of <see cref="MenuScrolling"/> layers.
///
/// Refactor notes: empty Start / Update removed; tuning constants exposed as
/// SerializeField; for-index loops avoid the IEnumerator allocation per call.
/// </summary>
public class TriggerMovement : MonoBehaviour
{
    [SerializeField] private List<PseudoMovimentoFogo> targets = new();
    [SerializeField] private PseudoAnimation psudeoAnimation;
    [SerializeField] private CanvasHandler ch;
    [SerializeField] private List<MenuScrolling> scrollings = new();

    [Header("Fire-intensity ramp")]
    [SerializeField] private float fireIntensityX = 0.4f;
    [SerializeField] private float fireIntensityY = 0.5f;
    [SerializeField] private float fireScaleX = 1.2f;
    [SerializeField] private float fireScaleY = 1.1f;
    [SerializeField] private Vector3 fireOffsetIncrease = new(-0.5f, 0f, 0f);
    [SerializeField] private Vector3 fireOffsetDecrease = new(0.5f, 0f, 0f);

    [Header("Scroll speed delta")]
    [SerializeField] private float scrollSpeedDelta = 2f;

    [Header("Menu fade")]
    [SerializeField] private float menuFadeDurationSeconds = 3f;

    private void TriggerMovementIncrease()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            var t = targets[i];
            if (t != null) t.changeIntensity(fireIntensityX, fireIntensityY, fireScaleX, fireScaleY, fireOffsetIncrease);
        }
        for (int i = 0; i < scrollings.Count; i++)
        {
            var s = scrollings[i];
            if (s != null) s.increaseSpeed(scrollSpeedDelta);
        }
    }

    private void TriggerMovementDecrease()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            var t = targets[i];
            if (t != null) t.changeIntensity(-fireIntensityX, -fireIntensityY, -fireScaleX, -fireScaleY, fireOffsetDecrease);
        }
        for (int i = 0; i < scrollings.Count; i++)
        {
            var s = scrollings[i];
            if (s != null) s.decreaseSpeed(scrollSpeedDelta);
        }
    }

    public void StartMenu(bool instant = false)
    {
        if (ch != null) StartCoroutine(ch.FadeIn(instant ? 0f : menuFadeDurationSeconds));
    }
}
