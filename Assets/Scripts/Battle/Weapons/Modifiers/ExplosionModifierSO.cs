using UnityEngine;

/// <summary>
/// Modifier that spawns an explosion prefab at the enemy's position when a
/// projectile hits. The explosion prefab handles its own collision damage via
/// <see cref="ExplosionModifier"/>.
/// </summary>
[CreateAssetMenu(menuName = "ProjectileModifiers/Explosion", fileName = "ExplosionModifierSO")]
public class ExplosionModifierSO : ModifierInterface
{
    [SerializeField] public GameObject explosionPrefab;
    [SerializeField] private float explosionDuration = 0.5f;

    [Tooltip("Damage the explosion deals to caught enemies (read by ExplosionModifier).")]
    [SerializeField] public float damage;

    public override void OnHitEnemy(ProjectileBehaviour projectile, Enemy enemy)
    {
        if (explosionPrefab == null || enemy == null) return;

        // Local var (not stored on the SO) — SO state is shared across scenes,
        // and we don't need to remember the spawned object after scheduling its destroy.
        var explosionObj = Object.Instantiate(explosionPrefab, enemy.transform.position, Quaternion.identity);
        Object.Destroy(explosionObj, explosionDuration);
    }
}
