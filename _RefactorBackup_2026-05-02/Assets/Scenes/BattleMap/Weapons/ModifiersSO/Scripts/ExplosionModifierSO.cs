using UnityEngine;

[CreateAssetMenu(menuName = "ProjectileModifiers/Explosion")]

public class ExplosionModifierSO : ModifierInterface
{
    public GameObject explosionPrefab;
    private GameObject explosionObj;
    [SerializeField] float explosionDuration;
    [SerializeField] public float damage;


    public override void OnHitEnemy(ProjectileBehaviour projectile, Enemy enemy)
    {
        if (explosionPrefab != null)
            explosionObj = Object.Instantiate(explosionPrefab, enemy.transform.position, Quaternion.identity);
            Destroy(explosionObj, explosionDuration);
    }

}
