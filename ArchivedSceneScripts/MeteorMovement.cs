using UnityEngine;

public class MeteorMovement : MonoBehaviour
{
    [SerializeField] Vector2 direction = new(-1, -1);
    [SerializeField] float speed = 5f;
    [Tooltip("Threshold for detecting a parent reset jump in world units")]
    [SerializeField] float resetJumpThreshold = 5f;

    Transform parentTf;
    Vector3 localStartPos;
    float lastParentX;

    void Awake()
    {
        if (direction.sqrMagnitude > 0.001f) direction = direction.normalized;
        parentTf = transform.parent;
        localStartPos = transform.position;
        lastParentX = parentTf != null ? parentTf.position.x : 0f;
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        if (parentTf == null) return;

        float currentParentX = parentTf.position.x;
        float delta = currentParentX - lastParentX;

        // If parent jumped right (big positive delta) - assume it looped/reset
        if (delta > resetJumpThreshold)
        {
            transform.position = localStartPos;
        }

        lastParentX = currentParentX;
    }
}
