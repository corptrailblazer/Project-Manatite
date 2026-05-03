using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class DraggableWeapon : MonoBehaviour
{
    private Camera cam;
    private bool isDragging = false;
    private Vector3 offset;
    public WeaponSO weaponData;
    public Vector2 startPosition;

    void Awake()
    {
        cam = Camera.main;
        startPosition = transform.position;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        // Quando segura o botão do mouse
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Pega posição do mouse em mundo
            Vector3 mouseWorld = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mouseWorld.z = 0;

            // Testa colisão com este objeto
            Collider2D col = GetComponent<Collider2D>();
            if (col == Physics2D.OverlapPoint(mouseWorld))
            {
                isDragging = true;
                offset = transform.position - mouseWorld;
            }
        }

        // Enquanto arrastando
        if (isDragging && Mouse.current.leftButton.isPressed)
        {
            Vector3 mouseWorld = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mouseWorld.z = 0;
            transform.position = mouseWorld + offset;
        }

        // Soltou o botão
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }
    }
}
