using ProjectManatite.Core;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Lets the player click-and-drag this object's transform with the mouse.
///
/// Refactor notes: <c>Camera.main</c> per-frame replaced with
/// <see cref="CameraCache.Main"/>; cached collider in <see cref="Awake"/>
/// instead of querying inside <c>Update</c>; null-safe against missing camera /
/// missing mouse device.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class DraggableWeapon : MonoBehaviour
{
    public WeaponSO weaponData;
    public Vector2 startPosition;

    private Camera cam;
    private Collider2D _col;
    private Transform _transform;
    private bool isDragging;
    private Vector3 offset;

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
        _transform = transform;
        startPosition = _transform.position;
    }

    private void Update()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        if (cam == null) cam = CameraCache.Main;
        if (cam == null) return;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            Vector3 mouseWorld = cam.ScreenToWorldPoint(mouse.position.ReadValue());
            mouseWorld.z = 0f;

            if (_col == Physics2D.OverlapPoint(mouseWorld))
            {
                isDragging = true;
                offset = _transform.position - mouseWorld;
            }
        }

        if (isDragging && mouse.leftButton.isPressed)
        {
            Vector3 mouseWorld = cam.ScreenToWorldPoint(mouse.position.ReadValue());
            mouseWorld.z = 0f;
            _transform.position = mouseWorld + offset;
        }

        if (mouse.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }
    }
}
