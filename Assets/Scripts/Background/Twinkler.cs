using UnityEngine;

/// <summary>
/// Scales the host transform with a sine wave to give a "twinkle" pulse.
/// Useful for stars and small accents in the menu background.
///
/// Class renamed from <c>SpritePulse</c> to match the file name. Cached
/// <see cref="Transform"/>.
/// </summary>
public class Twinkler : MonoBehaviour
{
    [Tooltip("Pulse rate (radians per second).")]
    [SerializeField] private float pulseSpeed = 2f;

    [Tooltip("Peak scale variation as a fraction of the base scale.")]
    [SerializeField] private float scaleAmount = 0.25f;

    private Transform _transform;
    private Vector3 baseScale;

    private void Awake()
    {
        _transform = transform;
        baseScale = _transform.localScale;
    }

    private void Update()
    {
        float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * scaleAmount;
        _transform.localScale = baseScale * scale;
    }
}
