using UnityEngine;

/// <summary>
/// Generic infinite-scroll background. Moves the transform along
/// <see cref="direction"/> at <see cref="speed"/> until it has travelled
/// <see cref="width"/> units, then snaps back to the start position.
///
/// Refactor notes: cached <see cref="Transform"/> reference avoids the Unity
/// native marshalling per Update.
/// </summary>
public class Scrolling : MonoBehaviour
{
    public float speed = 2f;
    public float width = 10f;
    public float offset = 0f;
    public Vector3 direction = Vector3.left;

    private Vector3 startPos;
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    private void Start()
    {
        startPos = _transform.position;
    }

    private void Update()
    {
        _transform.position += direction.normalized * (speed * Time.deltaTime);

        if (Vector3.Distance(startPos, _transform.position) >= width)
        {
            startPos.x += offset;
            width += offset;
            offset = 0f;
            _transform.position = startPos;
        }
    }
}
