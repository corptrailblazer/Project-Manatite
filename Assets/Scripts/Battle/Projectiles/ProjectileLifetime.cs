using ProjectManatite.Core;
using UnityEngine;

/// <summary>
/// Despawns the host GameObject after <see cref="lifetime"/> seconds.
///
/// Refactor note: the original used <c>Destroy(gameObject, lifetime)</c>, which
/// breaks pooled projectiles (the destroyed object would still have a queued
/// reference inside <see cref="PoolManager"/>). Now it returns to the pool when
/// available and falls back to Destroy otherwise.
/// </summary>
public class ProjectileLifetime : MonoBehaviour
{
    [Tooltip("Seconds before the projectile is despawned (or destroyed if not pooled).")]
    [Min(0)] public float lifetime = 5f;

    private float _enableTime;
    private PoolItem _poolItem;

    private void Awake()
    {
        TryGetComponent(out _poolItem);
    }

    private void OnEnable()
    {
        _enableTime = Time.time;
    }

    private void Update()
    {
        if (Time.time - _enableTime < lifetime) return;

        if (_poolItem != null && _poolItem.Manager != null)
            _poolItem.Manager.Despawn(gameObject);
        else
            Destroy(gameObject);
    }
}
