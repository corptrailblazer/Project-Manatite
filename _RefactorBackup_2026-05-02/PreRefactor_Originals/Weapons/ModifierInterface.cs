using UnityEngine;

public abstract class ModifierInterface : ScriptableObject
{
    // Called right after the projectile is spawned
    public virtual void OnProjectileSpawn(ProjectileBehaviour projectile) { }

    // Called when the projectile hits an enemy
    public virtual void OnHitEnemy(ProjectileBehaviour projectile, Enemy enemy) { }
}
