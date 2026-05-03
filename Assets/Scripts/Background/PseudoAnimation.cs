using UnityEngine;

/// <summary>
/// Vertical sine-wave bob with optional sub-pixel quantisation
/// (<see cref="pixelsPerUnit"/>). Used on the menu ship before the cutscene ends.
///
/// Refactor notes: cached transform, fixed <c>decreaseSpeed</c> which was
/// previously identical to <c>increaseSpeed</c> (both did <c>speed += amount</c>).
/// </summary>
public class PseudoAnimation : MonoBehaviour
{
    [Header("Configurações")]
    public float amplitude = 4f;
    public float speed = 2f;
    public int pixelsPerUnit = 32;

    private Vector3 startPos;
    private Animator animator;
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
        animator = GetComponent<Animator>();
        startPos = _transform.localPosition;
    }

    private void Update()
    {
        if (pixelsPerUnit <= 0) pixelsPerUnit = 1;
        float offset = (amplitude / pixelsPerUnit) * Mathf.Sin(Time.time * speed);
        _transform.localPosition = startPos + new Vector3(0f, offset, 0f);
    }

    public void OnAnimationEnd()
    {
        if (animator != null) animator.enabled = false;
        startPos = new Vector3(0f, 0.314f, 0f);
    }

    public void RemoveParticles()
    {
        if (TryGetComponent<ParticleSystem>(out var ps)) ps.Stop();
    }

    public void increaseSpeed(float delta) => speed += delta;
    public void decreaseSpeed(float delta) => speed -= delta;
}
