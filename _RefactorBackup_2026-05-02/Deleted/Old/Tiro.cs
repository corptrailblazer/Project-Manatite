using UnityEngine;
using UnityEngine.InputSystem;

public class CursorShooter2D_InputSystem : MonoBehaviour
{
    [Header("Disparo")]
    public GameObject projectilePrefab;
    public float fireInterval = 0.25f;
    public float projectileSpeed = 12f;
    public bool rotateShooter = true;

    [Header("Cone frontal")]
    public float AnguloRotacao = 90f;


    Camera cam;
    float timer;
    Vector2 original_facing;

    void Awake(){
        cam = Camera.main;
    } 

    void Update()
    {
        // posição do cursor (Input System)
        Vector2 screenPos = Mouse.current != null
            ? Mouse.current.position.ReadValue()
            : (Pointer.current != null ? Pointer.current.position.ReadValue() : Vector2.zero);

        if (screenPos == Vector2.zero && Mouse.current == null && Pointer.current == null) return;

        // mundo 2D
        Vector3 world = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, cam.nearClipPlane));
        world.z = transform.position.z;

        // direção até o cursor
        Vector2 dir = (world - transform.position).normalized;
        if (dir.sqrMagnitude < 1e-6f) return;

        // eixo "frente" do atirador (assumindo sprite aponta para +X)
        Vector2 forward = transform.parent.right;;

        // calcula ângulo entre forward e original_facing (sempre positivo)
        float angle = Vector2.Angle(dir, forward);

        // pega o "sinal" pelo cross product (Z do 2D)
        float cross = dir.x * forward.y - dir.y * forward.x;

        // aplica sinal
        if (cross > 0) angle = -angle;
        bool dentroDoCone = angle <= AnguloRotacao && angle > 0;

        if (rotateShooter && dentroDoCone) transform.right = dir;

        timer += Time.deltaTime;
        if (timer >= fireInterval)
        {
            Shoot(dir);
            timer = 0f;
        }
    }

    void Shoot(Vector2 _)
{
    // Direção baseada no "facing" do objeto (sprite apontando para +X)
    Vector2 facing = transform.right;

    // Rotação do projétil alinhada ao facing
    float ang = Mathf.Atan2(facing.y, facing.x) * Mathf.Rad2Deg;
    Quaternion rot = Quaternion.Euler(0f, 0f, ang);

    GameObject go = Instantiate(projectilePrefab, transform.position, rot);

    // Lança o projétil na direção que o objeto está virado
    var rb = go.GetComponent<Rigidbody2D>();
    if (rb != null)
        rb.linearVelocity = facing * projectileSpeed;
}

}
