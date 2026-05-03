using UnityEngine;

/// <summary>
/// Marker component preserved for backwards compatibility with prefabs that
/// reference it. Projectile collision logic lives in
/// <see cref="ProjectileBehaviour.OnTriggerEnter2D"/>; this class is intentionally
/// empty and exists only so the existing GUID stays resolvable.
/// </summary>
[DisallowMultipleComponent]
public class ProjectileCollision : MonoBehaviour
{
}
