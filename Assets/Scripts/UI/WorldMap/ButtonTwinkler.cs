using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Click-twinkle: scales a UI Button up then back down on every click.
/// Refactor notes: null-safe against missing Button component; simplified
/// scale-up/down into a single coroutine using the half-duration helper.
/// </summary>
[RequireComponent(typeof(Button))]
public class ButtonTwinkler : MonoBehaviour
{
    [SerializeField] private float scaleUpFactor = 1.2f;
    [SerializeField] private float animationDuration = 0.2f;

    private Button button;
    private Transform _transform;
    private Vector3 originalScale;

    private void Awake()
    {
        _transform = transform;
        button = GetComponent<Button>();
        originalScale = _transform.localScale;
        if (button != null) button.onClick.AddListener(() => StartCoroutine(Twinkle()));
    }

    public IEnumerator Twinkle()
    {
        Vector3 targetScale = originalScale * scaleUpFactor;
        float halfDuration = Mathf.Max(0.0001f, animationDuration * 0.5f);

        yield return ScaleRoutine(originalScale, targetScale, halfDuration);
        yield return ScaleRoutine(targetScale, originalScale, halfDuration);

        _transform.localScale = originalScale;
    }

    private IEnumerator ScaleRoutine(Vector3 from, Vector3 to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            _transform.localScale = Vector3.Lerp(from, to, t / duration);
            yield return null;
        }
    }
}
