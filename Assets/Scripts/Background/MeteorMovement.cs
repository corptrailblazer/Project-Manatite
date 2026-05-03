using UnityEngine;

/// <summary>
/// Diagonal-scrolling meteor with parent-loop detection. When the parent's X
/// jumps right (i.e. the parent looped to its starting position), this child
/// also resets so the meteor doesn't drift forever in world space.
///
/// Refactor notes: cached <see cref="Transform"/> and parent transform; minor
/// readability cleanup; null-safe against being unparented.
/// </summary>
public class MeteorMovement : MonoBehaviour
{
    [SerializeField] private Vector2 direction = new(-1, -1);
    [SerializeField] private float speed = 5f;

    [Tooltip("Threshold (world units) above which a positive parent-X delta is treated as a 'parent looped' event.")]
    [SerializeField] private float resetJumpThreshold = 5f;

    private Transform _transform;
    private Transform parentTf;
    private Vector3 localStartPos;
    private float lastParentX;

    private void Awake()
    {
        _transform = transform;
        parentTf = _transform.parent;

        if (direction.sqrMagnitude > 0.001f) direction = direction.normalized;
        localStartPos = _transform.position;
        lastParentX = parentTf != null ? parentTf.position.x : 0f;
    }

    private void Update()
    {
        _transform.position += (Vector3)(direction * (speed * Time.deltaTime));

        if (parentTf == null) return;

        float currentParentX = parentTf.position.x;
        float delta = currentParentX - lastParentX;
        if (delta > resetJumpThreshold) _transform.position = localStartPos;

        lastParentX = currentParentX;
    }
}
