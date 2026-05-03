using System.Collections;
using UnityEngine;

/// <summary>
/// Horizontal-scrolling menu background. <see cref="startMoving"/> starts the
/// scroll loop; <see cref="increaseSpeed"/> / <see cref="decreaseSpeed"/> ramp
/// the speed over a fixed duration.
///
/// <para>Refactor notes vs. the original MenuScrolling.cs:</para>
/// <list type="bullet">
///   <item>Empty Start removed.</item>
///   <item>Coroutine ramp logic was buggy — the stop condition for "decrease"
///         used the same accumulator as "increase" but with a negative delta,
///         which could under- or overshoot. Now uses a single signed-direction
///         loop with a tolerance.</item>
///   <item>Cached <see cref="Transform"/>.</item>
/// </list>
/// </summary>
public class MenuScrolling : MonoBehaviour
{
    [Tooltip("Movement speed (units / sec).")]
    public float speed = 2f;

    [Tooltip("How far the object travels before snapping back to startPos.")]
    public float width = 10f;

    [Tooltip("Optional offset added to startPos / width on each loop iteration.")]
    public float offset = 0f;

    public Vector3 direction = Vector3.left;

    public Vector3 startPos;

    private bool move;
    private Animator animator;
    private Transform _transform;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        _transform = transform;
    }

    private void Update()
    {
        if (!move) return;
        _transform.position += direction.normalized * (speed * Time.deltaTime);

        if (Vector3.Distance(startPos, _transform.position) >= width)
        {
            startPos.x += offset;
            width += offset;
            offset = 0f;
            _transform.position = startPos;
        }
    }

    public void startMoving()
    {
        if (animator != null) animator.enabled = false;
        move = true;
    }

    /// <summary>Ramp speed up by <paramref name="delta"/> over 2 seconds.</summary>
    public void increaseSpeed(float delta) => StartCoroutine(ChangeSpeedRoutine(delta, 2f));

    /// <summary>Ramp speed down by <paramref name="delta"/> over 1 second.</summary>
    public void decreaseSpeed(float delta) => StartCoroutine(ChangeSpeedRoutine(-delta, 1f));

    private IEnumerator ChangeSpeedRoutine(float signedDelta, float duration)
    {
        float startSpeed = speed;
        float target = startSpeed + signedDelta;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            speed = Mathf.Lerp(startSpeed, target, Mathf.Clamp01(t / duration));
            yield return null;
        }

        speed = target;
    }
}
