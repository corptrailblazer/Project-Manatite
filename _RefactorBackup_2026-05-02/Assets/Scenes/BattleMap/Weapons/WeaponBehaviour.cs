using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class WeaponBehaviour : MonoBehaviour
{
    float damage, fireRate, projSpeed;
    float timer;
    protected Transform enemy;
    public WeaponSO weapon;
    Vector2 fppos;
    Vector2 fpright;
    [SerializeField] LayerMask clickableMask = ~0;

    [SerializeField] GameObject projectilePrefab;
    [Header("Targeting")]
    [SerializeField] float detectRadius = 10f;
    [SerializeField, Range(0f, 180f)] float halfAngleDeg = 30f; // cone half-angle
    [SerializeField] LayerMask enemyMask; // set to "Enemy" layer
    
    [SerializeField] private TurretInventory turretInventory;
    
    Animator animator;
    private Transform fp;
    Collider2D col;
    private PoolManager pool;
    bool isInventoryActive = false;
    private List<ModifierInterface> modifiers = new List<ModifierInterface>();


    void Awake()
    {
        if (pool == null)
            pool = Object.FindAnyObjectByType<PoolManager>();

        fp = transform.Find("Firepoint");
        fppos = fp.position;
        fpright = fp.right;
        
        damage = weapon.damage;
        fireRate = weapon.fireRate;
        projSpeed = weapon.projSpeed;
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();

        
    }

    void Update()
    {
        if (fireRate <= 0 || projectilePrefab == null) return;

        timer += Time.deltaTime;
        if (timer >= 1f / fireRate)
        {
            var enemy = HasEnemyInArc(fppos, fpright);
            if (enemy != null)
            {
                timer = 0f;
                Shoot();
            }
        }

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        Collider2D hit = Physics2D.OverlapPoint(worldPos, clickableMask);

        if(hit != null && hit == col && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if(!isInventoryActive)
            {
                turretInventory.transform.localRotation = Quaternion.identity;
                turretInventory.transform.localPosition = turretInventory.placement;
                isInventoryActive = true;
            }
            else
            {
                turretInventory.transform.localRotation = Quaternion.identity;
                turretInventory.transform.localPosition = new Vector2 (50,50);
                isInventoryActive = false;
            }
            

        }
    }

    Transform HasEnemyInArc(Vector2 origin, Vector2 forward)
    {
        var hits = Physics2D.OverlapCircleAll(origin, detectRadius, enemyMask);
        if (hits == null || hits.Length == 0) return null;

        float cosThresh = Mathf.Cos(halfAngleDeg * Mathf.Deg2Rad);
        forward = forward.normalized;

        Vector2 turretPos = (Vector2)transform.position;

        foreach (var h in hits)
        {
            Vector2 to = (Vector2)h.transform.position - origin;
            float mag = to.magnitude;
            if (mag < 0.001f) continue;

            float dot = Vector2.Dot(forward, to / mag);
            if (dot >= cosThresh)
            {
                Vector2 toPivot = (Vector2)h.transform.position - turretPos;
                float targetAngle = Mathf.Atan2(toPivot.y, toPivot.x) * Mathf.Rad2Deg - 90;

                transform.rotation = Quaternion.Euler(0f, 0f, targetAngle); // snap
                fp.localRotation = Quaternion.Euler(0f, 0f, 90); // snap
                

                return h.transform;
            }
        }

        return null;
    }

    void Shoot()
    {
        if (pool == null) { Debug.LogError("[WeaponBehaviour] PoolManager is null."); return; }

        GameObject go = pool.Spawn(projectilePrefab, fp.position, transform.rotation);

        var rb = go.GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = (Vector2)fp.right * projSpeed;

        
        var proj = go.GetComponent<ProjectileBehaviour>();
        if (proj != null)
        {
            proj.damage = damage;
            proj.maxDistance = detectRadius;
            proj.acceleration = projSpeed;
            //proj.SetTarget(enemy); // THIS MAKES IT HOMING
            foreach (var mod in modifiers)
            {
                mod.OnProjectileSpawn(proj);

                // Subscribe dynamically
                proj.OnHitEnemy += (enemy) => mod.OnHitEnemy(proj, enemy);
            }

        }
        animator.speed = fireRate; // animation plays faster for higher fireRate
        animator.Play(animator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, 0f);

    }

    public void Apply(Chip chip)
    {
        damage += chip.chipSO.damageFlat;
        fireRate += chip.chipSO.fireRateFlat;
        projSpeed += chip.chipSO.projSpeedFlat;

        damage *= (1 + chip.chipSO.damagePct);
        fireRate *= (1 + chip.chipSO.fireRatePct);
        projSpeed *= (1 + chip.chipSO.projSpeedPct);

        var mod = chip.chipSO.modifier;
        if (mod != null)
            modifiers.Add(chip.chipSO.modifier);
    }

    public void Unapply(Chip chip)
    {
        damage -= chip.chipSO.damageFlat;
        fireRate -= chip.chipSO.fireRateFlat;
        projSpeed -= chip.chipSO.projSpeedFlat;

        damage /= (1 + chip.chipSO.damagePct);
        fireRate /= (1 + chip.chipSO.fireRatePct);
        projSpeed /= (1 + chip.chipSO.projSpeedPct);

        var mod = chip.chipSO.modifier;
        if (mod != null)
            modifiers.Remove(chip.chipSO.modifier);
    }

}
