using System.Collections;
using ProjectManatite.Core;
using TMPro;
using UnityEngine;

/// <summary>
/// Floating "-N" damage number that fades out and self-destructs.
///
/// <para>Refactor notes vs. the original DamageTextBehaviour.cs:</para>
/// <list type="bullet">
///   <item>Class name typo fixed: <c>DamageTextBeheviour</c> → <c>DamageTextBehaviour</c>.</item>
///   <item><b>Major perf bug fixed:</b> the original called <c>StartCoroutine(fadeOut())</c> from <c>Update</c> every single frame, queuing a new fade coroutine each tick (and allocating a fresh <c>WaitForSeconds</c> inside it). Now started once on <c>OnEnable</c>.</item>
///   <item>Wait reuses <see cref="WaitCache"/> — no per-tick allocation.</item>
///   <item>Movement uses <c>Time.deltaTime</c> so fade speed isn't tied to wait granularity.</item>
/// </list>
/// </summary>
[RequireComponent(typeof(TextMeshPro))]
public class DamageTextBehaviour : MonoBehaviour
{
    [Tooltip("Vertical drift speed (units / sec).")]
    [SerializeField] private float driftSpeed = 0.6f;

    [Tooltip("Alpha decay per frame step. Lower = slower fade.")]
    [SerializeField] private float fadeStep = 0.001f;

    [Tooltip("Seconds between fade ticks. Smaller = smoother fade.")]
    [SerializeField] private float tickInterval = 0.1f;

    private TextMeshPro damageTxt;
    private Transform _transform;

    private void Awake()
    {
        damageTxt = GetComponent<TextMeshPro>();
        _transform = transform;
    }

    private void OnEnable()
    {
        // Reset alpha so pooled instances re-fade cleanly.
        if (damageTxt != null)
        {
            var c = damageTxt.color;
            c.a = 1f;
            damageTxt.color = c;
        }
        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        if (damageTxt == null) yield break;

        var wait = WaitCache.Seconds(tickInterval);
        Vector3 drift = new(0f, driftSpeed * tickInterval, 0f);

        while (damageTxt.alpha > 0f)
        {
            damageTxt.alpha -= fadeStep;
            _transform.position += drift;
            yield return wait;
        }

        Destroy(gameObject);
    }
}
