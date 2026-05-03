using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour, IPoolable
{
    protected float hp;
    protected float speed;
    public float damage;
    int qntyMantatite;

    protected Transform player;
    protected Rigidbody2D rb;

    [Header("Chase Settings")]
    public string playerTag = "Player";
    public float acceleration = 10f;

    [Header("Wobble Settings")]
    [SerializeField] float wobbleSpeed = 1.5f;
    [SerializeField] float wobbleStrength = 0.8f;

    [SerializeField] private List<GameObject> dropTable = new List<GameObject>();
    [SerializeField] TextMeshPro damageTxt;

    [SerializeField] private GameObject deathExplosion;
    [SerializeField] private Coins coinManager;


    WinLevel WinLevel;

    public Action OnDeath;

    private PoolItem poolItem;

    public virtual void Init(EnemySO data)
    {
        hp = data.hp;
        speed = data.speed;
        damage = data.damage;
        qntyMantatite = data.qntyMantatite;

    }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        poolItem = GetComponent<PoolItem>(); // added
        coinManager = FindAnyObjectByType<Coins>();
        WinLevel = FindAnyObjectByType<WinLevel>();

    }

    protected virtual void FixedUpdate()
    {
        if (player == null) return;
        ChasePlayer();
    }

    protected virtual void ChasePlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        float noise = Mathf.PerlinNoise(Time.time * wobbleSpeed, GetInstanceID() * 0.2f) * 2f - 1f;
        Vector2 perp = new Vector2(-dir.y, dir.x);
        dir = (dir + perp * noise * wobbleStrength).normalized;

        Vector2 desiredVelocity = dir * speed;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, desiredVelocity, acceleration * Time.fixedDeltaTime);

        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 90f);
        }
    }

    public virtual void TakeDamage(float dmg)
    {
        hp -= dmg;
        rb.linearVelocity -= rb.linearVelocity * 5;
        showDamage(dmg);

        if (hp <= 0) Die();
    }

    protected virtual void Die()
    {
        OnDeath?.Invoke(); // tell spawner
        // return to pool if available; else destroy
        if (poolItem != null && poolItem.manager != null)
            poolItem.manager.Despawn(gameObject);
        else
            Destroy(gameObject);

        GameObject explosionObj = Instantiate(deathExplosion, transform.position, Quaternion.identity);
        Destroy(explosionObj, 0.3f);
        DropItem();
        WinLevel.IncreaseKillCount();
    }

    void DropItem()
    {
        GameObject randomItem = dropTable[UnityEngine.Random.Range(0, dropTable.Count)];
        if (UnityEngine.Random.Range(0f, 1f) < randomItem.GetComponent<Chip>().chipSO.dropRate)
            Instantiate(randomItem, transform.position, Quaternion.identity);
        coinManager.updateCoin(qntyMantatite);

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody != null)
        {
            if (other.gameObject.CompareTag(playerTag))
            {
                Die();
            }
                
        }
    }

    // ---------- IPoolable ----------
    public void OnSpawned()
    {

        if (rb) rb.linearVelocity = Vector2.zero;
        OnDeath = null;

        var p = GameObject.FindGameObjectWithTag(playerTag);
        if (p != null) player = p.transform;
        else
        {
            player = null;
        }

    }

    public void OnDespawned()
    {
        if (rb) rb.linearVelocity = Vector2.zero;
        player = null; // clear stale refs
    }

    void showDamage(float dmg)
    {
        TextMeshPro dmgText = Instantiate(damageTxt, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
        dmgText.text = dmg.ToString();

        //Destroy(dmgText);
    }

}
