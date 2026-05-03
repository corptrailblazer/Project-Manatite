using System;
using System.Collections.Generic;
using ProjectManatite.Core;
using TMPro;
using UnityEngine;

/// <summary>
/// Base enemy: chases the player with a Perlin-noise wobble, takes damage, drops loot
/// and explodes on death.
///
/// <para>Refactor notes vs. the original Enemy.cs:</para>
/// <list type="bullet">
///   <item>Implements <see cref="IDamageable"/>. Anything wanting to damage an enemy
///         can do so through the interface without referencing this class directly.</item>
///   <item>Direct <c>FindAnyObjectByType&lt;Coins&gt;()</c> / <c>FindAnyObjectByType&lt;WinLevel&gt;()</c>
///         calls were removed. Death and coin reward are now broadcast through
///         <see cref="GameEvents.EnemyDied"/> and <see cref="GameEvents.CoinsChanged"/>.
///         Coins.cs and WinLevel.cs subscribe.</item>
///   <item>Player lookup uses <see cref="GameObject.FindGameObjectWithTag"/> only on
///         <c>OnSpawned</c> — never per-frame.</item>
///   <item>The previous <c>Die()</c> ordering was buggy: it returned the object to
///         the pool *before* spawning the explosion / dropping items, which meant
///         <c>transform.position</c> was already neutral. Reordered.</item>
///   <item><c>damage</c> is now an exposed property; the underlying field is
///         private serialized.</item>
///   <item>Drop-table guards against an empty list and missing <c>Chip</c> components.</item>
/// </list>
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour, IPoolable, IDamageable
{
    // ---------- Serialized config ----------

    [Header("Chase")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float acceleration = 10f;

    [Header("Wobble")]
    [SerializeField] private float wobbleSpeed = 1.5f;
    [SerializeField] private float wobbleStrength = 0.8f;

    [Header("Loot / FX")]
    [SerializeField] private List<GameObject> dropTable = new();
    [SerializeField] private TextMeshPro damageTxt;
    [SerializeField] private GameObject deathExplosion;
    [SerializeField] private float explosionLifetime = 0.3f;
    [SerializeField] private Vector3 damageTextOffset = new(0, 2, 0);

    // ---------- Runtime stats (set by Init) ----------

    protected float hp;
    protected float speed;
    protected float damage;
    protected float maxHp;
    protected int qntyMantatite;

    // ---------- Cached components ----------

    protected Rigidbody2D rb;
    protected Transform player;
    private Transform _transform;
    private PoolItem _poolItem;

    // ---------- Public surface ----------

    /// <summary>Damage this enemy deals on contact.</summary>
    public float Damage => damage;

    /// <summary>Optional per-instance death callback (used by <see cref="EnemySpawner"/> for alive-counting).</summary>
    public Action OnDeath;

    // ---------- IDamageable ----------

    public float CurrentHealth => hp;
    public float MaxHealth => maxHp;
    public bool IsDead => hp <= 0;
    public event Action<float, float> OnHealthChanged;
    public event Action OnDied;

    // ---------- Init from SO ----------

    public virtual void Init(EnemySO data)
    {
        if (data == null) return;
        hp = data.hp;
        maxHp = data.hp;
        speed = data.speed;
        damage = data.damage;
        qntyMantatite = data.qntyMantatite;
    }

    // ---------- Lifecycle ----------

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _transform = transform;
        TryGetComponent(out _poolItem);
    }

    protected virtual void FixedUpdate()
    {
        if (player == null) return;
        ChasePlayer();
    }

    protected virtual void ChasePlayer()
    {
        Vector2 dir = ((Vector2)(player.position - _transform.position)).normalized;

        // Perlin-noise wobble in [-1, 1] perpendicular to chase direction.
        float noise = Mathf.PerlinNoise(Time.time * wobbleSpeed, GetInstanceID() * 0.2f) * 2f - 1f;
        Vector2 perp = new(-dir.y, dir.x);
        dir = (dir + perp * (noise * wobbleStrength)).normalized;

        Vector2 desiredVelocity = dir * speed;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, desiredVelocity, acceleration * Time.fixedDeltaTime);

        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            _transform.rotation = Quaternion.Euler(0, 0, angle + 90f);
        }
    }

    // ---------- Damage ----------

    public virtual void TakeDamage(float dmg, GameObject source = null)
    {
        if (IsDead || dmg <= 0) return;

        hp -= dmg;
        // Hit reaction: bounce back. v -= v*5 == v *= -4 (reverse direction, 4x speed).
        rb.linearVelocity *= -4f;
        ShowDamage(dmg);

        OnHealthChanged?.Invoke(hp, maxHp);

        if (IsDead) Die();
    }

    // Keep parameterless overload for callers that don't track a source.
    public void TakeDamage(float dmg) => TakeDamage(dmg, null);

    public void Heal(float amount)
    {
        if (amount <= 0 || IsDead) return;
        hp = Mathf.Min(hp + amount, maxHp);
        OnHealthChanged?.Invoke(hp, maxHp);
    }

    // ---------- Death ----------

    protected virtual void Die()
    {
        // Capture position BEFORE despawn — pooling moves us under the manager.
        Vector3 deathPos = _transform.position;

        OnDeath?.Invoke();
        OnDied?.Invoke();
        GameEvents.RaiseEnemyDied(gameObject);

        if (deathExplosion != null)
        {
            var explosion = Instantiate(deathExplosion, deathPos, Quaternion.identity);
            Destroy(explosion, explosionLifetime);
        }

        DropLoot(deathPos);

        // Return to pool last, so any subscribers that read state from us
        // (position, transform parent) get clean values.
        if (_poolItem != null && _poolItem.Manager != null)
            _poolItem.Manager.Despawn(gameObject);
        else
            Destroy(gameObject);
    }

    private void DropLoot(Vector3 worldPos)
    {
        if (dropTable.Count > 0)
        {
            var pick = dropTable[UnityEngine.Random.Range(0, dropTable.Count)];
            if (pick != null && pick.TryGetComponent<Chip>(out var chip) && chip.chipSO != null)
            {
                if (UnityEngine.Random.value < chip.chipSO.dropRate)
                {
                    Instantiate(pick, worldPos, Quaternion.identity);
                }
            }
        }

        if (qntyMantatite > 0)
            GameEvents.RaiseCoinsChanged(qntyMantatite);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody == null) return;
        if (other.gameObject.CompareTag(playerTag))
        {
            // Ramming the player kills the enemy outright (preserves original behaviour).
            Die();
        }
    }

    // ---------- IPoolable ----------

    public void OnSpawned()
    {
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // Reset HP signal subscribers / OnDeath invokers from a previous life.
        OnDeath = null;
        OnHealthChanged = null;
        OnDied = null;

        var p = GameObject.FindGameObjectWithTag(playerTag);
        player = p != null ? p.transform : null;

        GameEvents.RaiseEnemySpawned(gameObject);
    }

    public void OnDespawned()
    {
        if (rb != null) rb.linearVelocity = Vector2.zero;
        player = null;
    }

    // ---------- Damage text ----------

    private void ShowDamage(float dmg)
    {
        if (damageTxt == null) return;
        var text = Instantiate(damageTxt, _transform.position + damageTextOffset, Quaternion.identity);
        text.text = dmg.ToString("0");
    }
}
