using System.Collections.Generic;
using ProjectManatite.Core;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// A turret. Detects enemies in a forward cone, fires pooled projectiles at them,
/// and applies / unapplies chip modifiers when the player drags chips into a slot.
///
/// <para>Refactor notes vs. the original WeaponBehaviour.cs:</para>
/// <list type="bullet">
///   <item>Implements <see cref="IEquippable"/> so chip code can apply mods through the interface.</item>
///   <item><c>Physics2D.OverlapCircleAll</c> (which allocates a fresh Collider2D[] every fire) replaced with <c>OverlapCircleNonAlloc</c> against a shared buffer in <see cref="Physics2DBuffers"/>.</item>
///   <item><c>Camera.main</c> per-frame lookup replaced with <see cref="CameraCache.Main"/>.</item>
///   <item><c>FindAnyObjectByType&lt;PoolManager&gt;()</c> kept only as a fallback — drop the reference into <see cref="_poolOverride"/> in the inspector to skip the search.</item>
///   <item>Fire timer is now reset every <c>1/fireRate</c> regardless of whether an enemy is in arc — the original code only reset on a successful shot, causing accumulated drift after long no-target periods.</item>
///   <item><see cref="Unapply"/> is now division-safe: a chip with a -100% multiplier (which would divide by zero on removal) is handled.</item>
///   <item>Inventory toggle extracted into <see cref="ToggleTurretInventoryPanel"/> for readability.</item>
/// </list>
/// </summary>
public class WeaponBehaviour : MonoBehaviour, IEquippable
{
    // ---------- Serialized config ----------

    [Header("Weapon data")]
    public WeaponSO weapon;
    [SerializeField] private GameObject projectilePrefab;

    [Header("Targeting")]
    [SerializeField] private float detectRadius = 10f;
    [Tooltip("Half of the firing-cone angle (the full cone is 2x this).")]
    [SerializeField, Range(0f, 180f)] private float halfAngleDeg = 30f;
    [SerializeField] private LayerMask enemyMask;

    [Header("UI")]
    [SerializeField] private LayerMask clickableMask = ~0;
    [SerializeField] private TurretInventory turretInventory;
    [Tooltip("Where the inventory popup hides off-screen when closed.")]
    [SerializeField] private Vector2 hiddenInventoryPos = new(50, 50);

    [Header("Pool")]
    [Tooltip("Optional. If left null, the first PoolManager in the scene will be found at Awake.")]
    [SerializeField] private PoolManager _poolOverride;

    // ---------- Runtime state ----------

    private float damage;
    private float fireRate;
    private float projSpeed;
    private float timer;

    private Animator animator;
    private Collider2D col;
    private Transform fp;            // Firepoint child
    private PoolManager pool;
    private Camera _cam;
    private Transform _transform;

    private bool isInventoryActive;
    private readonly List<ModifierInterface> modifiers = new();

    // ---------- Lifecycle ----------

    private void Awake()
    {
        _transform = transform;
        pool = _poolOverride != null ? _poolOverride : Object.FindAnyObjectByType<PoolManager>();

        fp = _transform.Find("Firepoint");

        if (weapon != null)
        {
            damage = weapon.damage;
            fireRate = weapon.fireRate;
            projSpeed = weapon.projSpeed;
        }

        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    private void Update()
    {
        TickFiring();
        TickClickToToggleInventory();
    }

    // ---------- Firing ----------

    private void TickFiring()
    {
        if (fireRate <= 0f || projectilePrefab == null || fp == null) return;

        timer += Time.deltaTime;
        float interval = 1f / fireRate;
        if (timer < interval) return;

        // Always reset the timer when the cooldown elapses, regardless of whether
        // we found a target — otherwise a turret with no enemies in front would
        // accumulate timer to huge values, then "burst fire" when one arrived.
        timer = 0f;

        var target = HasEnemyInArc(fp.position, fp.right);
        if (target != null) Shoot();
    }

    private Transform HasEnemyInArc(Vector2 origin, Vector2 forward)
    {
        // NonAlloc against a shared buffer — replaces the per-call Collider2D[] allocation.
        var buffer = Physics2DBuffers.GetColliderBuffer();
        int count = Physics2D.OverlapCircleNonAlloc(origin, detectRadius, buffer, enemyMask);
        if (count == 0) return null;

        float cosThresh = Mathf.Cos(halfAngleDeg * Mathf.Deg2Rad);
        forward = forward.normalized;
        Vector2 turretPos = _transform.position;

        for (int i = 0; i < count; i++)
        {
            var h = buffer[i];
            if (h == null) continue;

            Vector2 to = (Vector2)h.transform.position - origin;
            float mag = to.magnitude;
            if (mag < 0.001f) continue;

            float dot = Vector2.Dot(forward, to / mag);
            if (dot < cosThresh) continue;

            // Aim: rotate the turret towards the target, lock the firepoint forward.
            Vector2 toPivot = (Vector2)h.transform.position - turretPos;
            float targetAngle = Mathf.Atan2(toPivot.y, toPivot.x) * Mathf.Rad2Deg - 90f;

            _transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
            if (fp != null) fp.localRotation = Quaternion.Euler(0f, 0f, 90f);
            return h.transform;
        }

        return null;
    }

    private void Shoot()
    {
        if (pool == null || fp == null)
        {
            Debug.LogError("[WeaponBehaviour] PoolManager is null.", this);
            return;
        }

        GameObject go = pool.Spawn(projectilePrefab, fp.position, _transform.rotation);
        if (go == null) return;

        if (go.TryGetComponent<Rigidbody2D>(out var rb))
            rb.linearVelocity = (Vector2)fp.right * projSpeed;

        if (go.TryGetComponent<ProjectileBehaviour>(out var proj))
        {
            proj.SetStats(damage, projSpeed, detectRadius);

            // Wire each modifier into this projectile. Modifiers may subscribe to
            // OnHitEnemy via the lambda; OnDespawned clears the event so closures
            // don't leak into the pooled instance's next life.
            for (int i = 0; i < modifiers.Count; i++)
            {
                var mod = modifiers[i];
                if (mod == null) continue;
                mod.OnProjectileSpawn(proj);
                proj.OnHitEnemy += enemy => mod.OnHitEnemy(proj, enemy);
            }
        }

        if (animator != null)
        {
            animator.speed = fireRate;
            animator.Play(animator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, 0f);
        }
    }

    // ---------- Click-to-toggle inventory ----------

    private void TickClickToToggleInventory()
    {
        if (turretInventory == null || col == null) return;
        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        if (_cam == null) _cam = CameraCache.Main;
        if (_cam == null) return;

        Vector2 worldPos = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        var hit = Physics2D.OverlapPoint(worldPos, clickableMask);
        if (hit != col) return;

        ToggleTurretInventoryPanel();
    }

    private void ToggleTurretInventoryPanel()
    {
        var ti = turretInventory.transform;
        ti.localRotation = Quaternion.identity;
        ti.localPosition = isInventoryActive ? (Vector3)hiddenInventoryPos : (Vector3)turretInventory.placement;
        isInventoryActive = !isInventoryActive;
    }

    // ---------- IEquippable / chip math ----------

    public void Apply(ChipSO chip) => Apply(chip == null ? null : new ChipApplyArgs(chip));

    public void Apply(Chip chip)
    {
        if (chip == null || chip.chipSO == null) return;
        Apply(new ChipApplyArgs(chip.chipSO));
    }

    public void Unapply(ChipSO chip) => Unapply(chip == null ? null : new ChipApplyArgs(chip));

    public void Unapply(Chip chip)
    {
        if (chip == null || chip.chipSO == null) return;
        Unapply(new ChipApplyArgs(chip.chipSO));
    }

    private void Apply(ChipApplyArgs args)
    {
        if (args == null) return;
        damage    = (damage    + args.damageFlat)    * (1f + args.damagePct);
        fireRate  = (fireRate  + args.fireRateFlat)  * (1f + args.fireRatePct);
        projSpeed = (projSpeed + args.projSpeedFlat) * (1f + args.projSpeedPct);

        if (args.modifier != null) modifiers.Add(args.modifier);
    }

    private void Unapply(ChipApplyArgs args)
    {
        if (args == null) return;
        // Inverse of Apply: divide pct first, then subtract flat. Guard against
        // the (1+pct) == 0 case which would divide by zero.
        damage    = SafeDivide(damage,    1f + args.damagePct)    - args.damageFlat;
        fireRate  = SafeDivide(fireRate,  1f + args.fireRatePct)  - args.fireRateFlat;
        projSpeed = SafeDivide(projSpeed, 1f + args.projSpeedPct) - args.projSpeedFlat;

        if (args.modifier != null) modifiers.Remove(args.modifier);
    }

    private static float SafeDivide(float a, float b) =>
        Mathf.Abs(b) < 0.0001f ? a : a / b;

    // Lightweight bag of the chip stats we actually care about. Decouples the
    // Apply / Unapply math from where the values came from (ChipSO directly,
    // a chip MonoBehaviour, future save data...).
    private sealed class ChipApplyArgs
    {
        public readonly float damageFlat, damagePct;
        public readonly float fireRateFlat, fireRatePct;
        public readonly float projSpeedFlat, projSpeedPct;
        public readonly ModifierInterface modifier;

        public ChipApplyArgs(ChipSO so)
        {
            damageFlat = so.damageFlat;       damagePct = so.damagePct;
            fireRateFlat = so.fireRateFlat;   fireRatePct = so.fireRatePct;
            projSpeedFlat = so.projSpeedFlat; projSpeedPct = so.projSpeedPct;
            modifier = so.modifier;
        }
    }
}
