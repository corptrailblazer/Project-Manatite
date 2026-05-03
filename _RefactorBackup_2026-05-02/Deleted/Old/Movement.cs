using UnityEngine;
using UnityEngine.InputSystem; // novo Input System

public class OrbitAround : MonoBehaviour
{
    [Header("Orbit Settings")]
    public Transform target;
    public float orbitSpeedNormal = 50f;
    public float orbitSpeedBoost = 150f;
    public float orbitRadius = 2f;

    [Header("Acceleration Settings")]
    public float accelerationRate = 5f;

    private float currentOrbitSpeed;
    private float angle = 0f;
    private float targetSize = 0f;
    float targetSpeed = 0f;

    // Input Action (crie via Input Actions asset ou manualmente)
    public InputActionReference forward;
    public InputActionReference backward;

    void OnEnable()
    {
        forward.action.Enable();
        backward.action.Enable();
    }

    void OnDisable()
    {
        forward.action.Disable();
        backward.action.Disable();
    }

    void Start()
    {
        currentOrbitSpeed = orbitSpeedNormal;

        if (target != null)
        {
            Renderer rend = target.GetComponent<Renderer>();
            if (rend != null)
            {
                Vector3 extents = rend.bounds.extents;
                targetSize = Mathf.Max(extents.x, extents.y, extents.z);
            }
        }
    }

    void Update()
    {
        if (target == null) return;

        if (forward.action.IsPressed())
            targetSpeed = orbitSpeedBoost;

        if (backward.action.IsPressed())
            targetSpeed = -orbitSpeedBoost;

        if ((!forward.action.IsPressed() && !backward.action.IsPressed()) || (forward.action.IsPressed() && backward.action.IsPressed()))
            targetSpeed = orbitSpeedNormal;

        // Suaviza a transição
        currentOrbitSpeed = Mathf.Lerp(currentOrbitSpeed, targetSpeed, Time.deltaTime * accelerationRate);

        // Atualiza posição e rotação
        angle -= currentOrbitSpeed * Time.deltaTime;

        float rad = angle * Mathf.Deg2Rad;
        float totalRadius = targetSize + orbitRadius;

        Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * totalRadius;
        transform.position = target.position + offset;

        Vector3 tangent = new Vector3(Mathf.Sin(rad), -Mathf.Cos(rad), 0);
        float angleDeg = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angleDeg);
    }
}
