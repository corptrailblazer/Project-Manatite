using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Scales a UI element on pointer-enter / shrinks on pointer-exit.
/// Already event-driven (no per-frame input polling); the only refactor is
/// caching <see cref="Transform"/> and a small null-safety pass.
/// </summary>
public class UIButtonHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float hoverScale = 1.1f;

    [Tooltip("Higher = snappier scale interpolation.")]
    [SerializeField] private float speed = 12f;

    private Transform _transform;
    private Vector3 baseScale;
    private Vector3 targetScale;

    private void Awake()
    {
        _transform = transform;
        baseScale = _transform.localScale;
        targetScale = baseScale;
    }

    private void Update()
    {
        _transform.localScale = Vector3.Lerp(
            _transform.localScale,
            targetScale,
            Time.unscaledDeltaTime * speed);
    }

    public void OnPointerEnter(PointerEventData eventData) => targetScale = baseScale * hoverScale;
    public void OnPointerExit(PointerEventData eventData)  => targetScale = baseScale;
}
