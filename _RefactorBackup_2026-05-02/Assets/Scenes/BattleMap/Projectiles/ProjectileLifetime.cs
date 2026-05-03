using UnityEngine;

public class ProjectileLifetime : MonoBehaviour
{
    [Tooltip("Tempo de vida em segundos antes de destruir o projétil")]
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
