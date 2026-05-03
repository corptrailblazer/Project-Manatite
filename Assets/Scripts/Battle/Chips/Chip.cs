using System.Collections;
using ProjectManatite.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

/// <summary>
/// A chip pickup. Falls from the sky when an enemy drops it; clicking it places
/// it in the <see cref="Inventory"/>; clicking it again selects/deselects so a
/// turret slot can equip it.
///
/// <para>Refactor notes vs. the original Chip.cs:</para>
/// <list type="bullet">
///   <item><c>Camera.main</c> per frame replaced with <see cref="CameraCache.Main"/>.</item>
///   <item>FindAnyObjectByType for <c>Inventory</c> / <c>SaveLoader</c> remain only as fallbacks (drag refs in the inspector to skip the search). Save-on-collect is now broadcast via <see cref="GameEvents.SaveRequested"/>.</item>
///   <item>Tooltip is instantiated once and toggled, instead of being destroyed every frame the cursor moves off. The original "<c>childCount &gt; 0</c>" hack incorrectly assumed the chip had no other children.</item>
///   <item>Coroutine waits use <see cref="WaitCache"/> to avoid per-call <c>WaitForSeconds</c> allocations.</item>
///   <item>Removed: dead drag/release commented block, unused <c>setEquipped</c> coroutine, redundant <c>GetComponent&lt;Tooltip&gt;()</c> on an already-typed reference.</item>
/// </list>
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Chip : MonoBehaviour
{
    // ---------- Serialized config ----------

    [Header("Falling")]
    [SerializeField, FormerlySerializedAs("fall_speed")] private float fallSpeed = 1f;

    [Header("References")]
    [Tooltip("Optional. If null, the first Inventory in the scene is used.")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private LayerMask clickableMask = ~0;
    [SerializeField] private Tooltip tooltipPrefab;

    [Header("Lifetime")]
    [Tooltip("Auto-destroy after this many seconds if not collected.")]
    [SerializeField] private float autoDestroyAfterSeconds = 30f;
    [SerializeField] private float collectToInventoryDelay = 0.1f;
    [SerializeField] private float collectToSaveDelay = 1f;

    // ---------- Public state read by other systems ----------

    /// <summary>The stat block. Read by Tooltip, Inventory, WeaponBehaviour, EnemyDropTable.</summary>
    public ChipSO chipSO;

    /// <summary>True once the chip is parked in an inventory slot.</summary>
    public bool isInventory;

    /// <summary>True when highlighted for equipping. Set/cleared by <see cref="Select"/> / <see cref="Unselect"/>.</summary>
    public bool isSelected;

    /// <summary>The slot this chip is currently equipped on, or <c>null</c> if free.</summary>
    public SlotBehaviour equippedSlot;

    // ---------- Cached components ----------

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Collider2D _col;
    private Tooltip _tooltip;
    private Camera _cam;
    private Transform _transform;

    /// <summary>Public collider reference for click-test comparison from other systems.</summary>
    public Collider2D col => _col;

    private static readonly Color SelectedTint   = new(0.5f, 0.5f, 0.5f, 1f);
    private static readonly Color UnselectedTint = Color.white;

    // ---------- Lifecycle ----------

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
        _transform = transform;

        if (inventory == null)
            inventory = Object.FindAnyObjectByType<Inventory>();
    }

    private void Start()
    {
        rb.linearVelocity = new Vector2(0f, -fallSpeed);
        StartCoroutine(AutoDestroyRoutine());
    }

    private void Update()
    {
        if (Mouse.current == null) return;

        if (_cam == null) _cam = CameraCache.Main;
        if (_cam == null) return;

        Vector2 worldPos = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Collider2D hit = Physics2D.OverlapPoint(worldPos, clickableMask);
        bool clicked = Mouse.current.leftButton.wasPressedThisFrame;
        bool hoveringMe = hit == _col;

        if (equippedSlot == null)
        {
            if (clicked && !isInventory) Collect(hit);
            else if (clicked && isInventory) Select(hit);
        }
        else if (clicked && hoveringMe)
        {
            equippedSlot.Unequip();
        }

        SetHover(hoveringMe);
    }

    // ---------- Collect ----------

    private void Collect(Collider2D hit)
    {
        if (hit != _col) return;
        DoCollect(requestSave: true);
    }

    /// <summary>Public entry used by <see cref="SaveLoader"/> when restoring a saved chip.</summary>
    public bool Collect(bool requestSave = true)
    {
        return DoCollect(requestSave);
    }

    private bool DoCollect(bool requestSave)
    {
        if (inventory == null) return false;
        if (!inventory.Collect(this)) return false;

        StartCoroutine(SetInventoryRoutine(requestSave));
        GameEvents.RaiseChipCollected(this);
        return true;
    }

    // ---------- Selection ----------

    private void Select(Collider2D hit)
    {
        if (hit != _col || inventory == null) return;

        if (!isSelected)
        {
            isSelected = true;
            inventory.ChipSelected = this;
            sr.color = SelectedTint;
            GameEvents.RaiseChipSelected(this);
        }
        else
        {
            Unselect();
        }
    }

    public void Unselect()
    {
        isSelected = false;
        if (inventory != null) inventory.ChipSelected = null;
        sr.color = UnselectedTint;
    }

    // ---------- Hover / Tooltip ----------

    private void SetHover(bool hovering)
    {
        if (hovering) ShowTooltip();
        else HideTooltip();
    }

    private void ShowTooltip()
    {
        if (tooltipPrefab == null) return;
        if (_tooltip == null)
        {
            _tooltip = Instantiate(tooltipPrefab, _transform);
            _tooltip.updateInfo(this);
        }
        _tooltip.gameObject.SetActive(true);
        var t = _tooltip.transform;
        t.localRotation = Quaternion.identity;
        t.localPosition = _tooltip.toolPlacement;
    }

    private void HideTooltip()
    {
        if (_tooltip != null && _tooltip.gameObject.activeSelf)
            _tooltip.gameObject.SetActive(false);
    }

    // ---------- Coroutines ----------

    private IEnumerator SetInventoryRoutine(bool requestSave)
    {
        yield return WaitCache.Seconds(collectToInventoryDelay);
        isInventory = true;

        if (requestSave)
        {
            yield return WaitCache.Seconds(collectToSaveDelay);
            GameEvents.RaiseSaveRequested();
        }
    }

    private IEnumerator AutoDestroyRoutine()
    {
        yield return WaitCache.Seconds(autoDestroyAfterSeconds);
        if (!isInventory) Destroy(gameObject);
    }
}
