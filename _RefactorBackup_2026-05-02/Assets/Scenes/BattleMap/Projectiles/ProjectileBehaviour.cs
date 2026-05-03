using UnityEngine;
using System.Collections.Generic;
using System;


public class ProjectileBehaviour : MonoBehaviour, IPoolable
{
    protected Transform enemy;
    protected Rigidbody2D rb;
    public float acceleration = 10f;
    public float damage;
    private Vector2 spawnPos;
    public float maxDistance = 10f; // set at spawn
    public List<string> hittableTargets = new List<string>();
    public event Action<Enemy> OnHitEnemy;
    

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spawnPos = transform.position; // original behavior kept
    }

    void OnEnable()
    {
        ResetState(); // ensures correct reset when reused from pool
    }

    public void SetTarget(Transform target)
    {
        enemy = target;
    }

    protected virtual void FixedUpdate()
    {
        
        ChaseEnemy();

        if (Vector2.Distance(spawnPos, transform.position) >= maxDistance)
        {
            Die();
            return;
        }
    }

    protected virtual void ChaseEnemy()
    {

        var t = enemy;           // local copy prevents race with destruction
        if (!t) return;
        
        Vector2 dir = (enemy.position - transform.position).normalized;
        Vector2 desiredVelocity = dir * rb.linearVelocity.magnitude;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, desiredVelocity, acceleration * Time.fixedDeltaTime);

        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public virtual void Die()
    {
        var pi = GetComponent<PoolItem>();
        if (pi != null && pi.manager != null) pi.manager.Despawn(gameObject);
        else Destroy(gameObject); // fallback if not pooled
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody != null)
        {
            if (hittableTargets.Contains(other.gameObject.tag))
            {

                var enemy = other.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage); // same here
                    OnHitEnemy?.Invoke(enemy);
                }

                var p = GetComponent<ProjectileBehaviour>();
                if (p) p.Die(); else Destroy(gameObject);

            }

            
        }
    }

    public void OnSpawned()   { ResetState(); }
    public void OnDespawned() 
    {
        if (rb) rb.linearVelocity = Vector2.zero;
        OnHitEnemy = null;
    }

    protected virtual void ResetState()
    {
        spawnPos = transform.position; // re-arm distance for pooled reuse
        // shooter sets velocity/rotation right after Spawn
    }
}
