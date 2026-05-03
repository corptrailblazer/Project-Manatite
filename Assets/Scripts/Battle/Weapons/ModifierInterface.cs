using UnityEngine;

/// <summary>
/// Base ScriptableObject for projectile / weapon side-effects.
/// Concrete modifiers (e.g. <see cref="ExplosionModifierSO"/>) override the hooks they care about.
///
/// Naming kept as <c>ModifierInterface</c> for compatibility with existing chip
/// SO assets that reference this type by GUID — despite being an abstract class,
/// not a C# interface.
/// </summary>
public abstract class ModifierInterface : ScriptableObject
{
    /// <summary>Called immediately after a projectile is spawned.</summary>
    public virtual void OnProjectileSpawn(ProjectileBehaviour projectile) { }

    /// <summary>Called when a projectile reports a hit on an enemy (after damage is applied).</summary>
    public virtual void OnHitEnemy(ProjectileBehaviour projectile, Enemy enemy) { }
}
