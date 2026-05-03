using UnityEngine;

/// <summary>
/// Procedural fire-flicker via two-axis Perlin-noise scale modulation.
/// Optionally randomises noise offsets so multiple fires don't pulse in sync.
///
/// Class name kept (Portuguese: "fake fire movement") to preserve scene/prefab
/// references via GUID. Cached transform.
/// </summary>
public class PseudoMovimentoFogo : MonoBehaviour
{
    [Header("Distortion")]
    public float baseScaleX = 1f;
    public float baseScaleY = 1f;
    public float amplitudeX = 0.2f;
    public float amplitudeY = 0.3f;
    public float speedX = 1.5f;
    public float speedY = 2f;
    public bool randomizeOffset = true;

    private float offsetX;
    private float offsetY;
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    private void Start()
    {
        offsetX = randomizeOffset ? Random.Range(0f, 100f) : 0f;
        offsetY = randomizeOffset ? Random.Range(0f, 100f) : 10f;
    }

    private void Update()
    {
        float noiseX = (Mathf.PerlinNoise(Time.time * speedX + offsetX, 0f) * 2f - 1f) * amplitudeX;
        float noiseY = (Mathf.PerlinNoise(Time.time * speedY + offsetY, 0f) * 2f - 1f) * amplitudeY;

        Vector3 scale = _transform.localScale;
        scale.x = baseScaleX + noiseX;
        scale.y = baseScaleY + noiseY;
        _transform.localScale = scale;
    }

    public void changeIntensity(float x, float y, float scaleX, float scaleY, Vector3 offset)
    {
        amplitudeX += x;
        amplitudeY += y;
        baseScaleX += scaleX;
        baseScaleY += scaleY;
        _transform.localPosition += offset;
    }
}
