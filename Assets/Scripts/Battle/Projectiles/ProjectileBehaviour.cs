using System;
using System.Collections.Generic;
using ProjectManatite.Core;
using UnityEngine;

/// <summary>
/// Pooled projectile. Optionally homes towards a target, despawns after travelling
/// <see cref="maxDistance"/> from its spawn point, and damages enemies it overlaps.
///
/// <para>Refactor notes vs. the original ProjectileBehaviour.cs:</para>
/// <list type="bullet">
///   <item>Talks to <see cref="IDamageable"/> instead of <c>Enemy.TakeDamage</c> directly. Future destructibles can be hit without changes here.</item>
///   <item>Cached <c>transform</c> reference avoids the Unity native marshalling on every <c>FixedUpdate</c>.</item>
///   <item><see cref="SetStats"/> replaces three public-field assignments from <c>WeaponBehaviour</c> — keeps fields private and intent clear.</item>
///   <item>Removed the redundant <c>GetComponent&lt;ProjectileBehaviour&gt;()</c> lookup inside <c>OnTriggerEnter2D</c> (we already <i>are</i> a ProjectileBehaviour).</item>
/// </list>
/// </summary>
public class ProjectileBehaviour : MonoBehaviour, IPoolable
{
    [Header("Targeting")]
    [Tooltip("If set, the projectile homes towards this transform.")]
    protected Transform enemy;

    [Header("Movement")]
    [SerializeField] public float acceleration = 10f;
    [SerializeField] public float damage;
    [SerializeField] public float maxDistance = 10f;

    [Tooltip("Tags this projectile can damage. Anything else is ignored on collision.")]
    [SerializeField] public List<string> hittableTargets = new();

    /// <summary>Raised once per hit. Modifiers (e.g. explosions) subscribe.</summary>
    public event Action<Enemy> OnHitEnemy;

    // Cached components / state
    protected Rigidbody2D rb;
    private Transform _transform;
    private Vector2 spawnPos;

    // ---------- Lifecycle ----------

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _transform = transform;
        spawnPos = _transform.position;
    }

    private void OnEnable() => ResetState();

    public void SetTarget(Transform target) => enemy = target;

    /// <summary>
    /// Set per-shot stats from the firing weapon. Avoids exposing public fields
    /// for these and lets us validate / clamp later.
    /// </summary>
    public void SetStats(float damage, float acceleration, float maxDistance)
    {
        this.damage = damage;
        this.acceleration = acceleration;
        this.maxDistance = maxDistance;
    }

    protected virtual void FixedUpdate()
    {
        ChaseEnemy();

        // Self-destruct after travelling max distance from spawn — keeps stray
        // projectiles from living forever in the pool.
        if (((Vector2)_transform.position - spawnPos).sqrMagnitude >= maxDistance * maxDistance)
        {
            Die();
        }
    }

    protected virtual void ChaseEnemy()
    {
        var target = enemy; // local copy in case enemy is destroyed mid-frame
        if (target == null) return;

        Vector2 dir = ((Vector2)(target.position - _transform.position)).normalized;
        Vector2 desiredVelocity = dir * rb.linearVelocity.magnitude;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, desiredVelocity, acceleration * Time.fixedDeltaTime);

        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            _transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public virtual void Die()
    {
        if (TryGetComponent<PoolItem>(out var pi) && pi.Manager != null)
            pi.Manager.Despawn(gameObject);
        else
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hittableTargets.Contains(other.gameObject.tag)) return;

        // Apply damage via IDamageable so the projectile doesn't need to know
        // about every concrete enemy / destructible class.
        if (other.TryGetComponent<IDamageable>(out var damageable))
            damageable.TakeDamage(damage, gameObject);

        // Modifiers expect a concrete Enemy — preserve the old call.
        if (other.TryGetComponent<Enemy>(out var enemyHit))
            OnHitEnemy?.Invoke(enemyHit);

        Die();
    }

    // ---------- IPoolable ----------

    public void OnSpawned() => ResetState();

    public void OnDespawned()
    {
        if (rb != null) rb.linearVelocity = Vector2.zero;
        OnHitEnemy = null;
        enemy = null;
    }

    protected virtual void ResetState()
    {
        if (_transform == null) _transform = transform;
        spawnPos = _transform.position;
    }
}
