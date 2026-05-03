using ProjectManatite.Core;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// A single equipment slot on a turret. Detects clicks while a chip is selected
/// in the <see cref="Inventory"/>, equips the selected chip, applies its modifiers
/// to the turret, and broadcasts a save request.
///
/// <para>Refactor notes vs. the original SlotBehaviour.cs:</para>
/// <list type="bullet">
///   <item>Removed the long commented-out trigger-based implementation.</item>
///   <item><c>Camera.main</c> per frame replaced with <see cref="CameraCache.Main"/>.</item>
///   <item><c>FindAnyObjectByType&lt;SaveLoader&gt;()</c> at <c>Start</c> retained as a fallback only — the Save call now also fires <see cref="GameEvents.SaveRequested"/> so other listeners can react.</item>
///   <item>Equip / Unequip emit chip-equipped events for analytics / VFX subscribers.</item>
///   <item>Null-checks added for <see cref="Inventory.ChipSelected"/> and <c>occupiedChip</c>.</item>
/// </list>
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class SlotBehaviour : MonoBehaviour
{
    [SerializeField] private LayerMask clickableMask = ~0;
    [SerializeField] private Inventory inventory;

    [Tooltip("Optional. If null, the first SaveLoader in the scene is used; if absent, a SaveRequested event is raised instead.")]
    [SerializeField] private SaveLoader saveLoader;

    /// <summary>Chip currently equipped here (or null).</summary>
    public Chip occupiedChip;

    private TurretInventory turretInventory;
    private Collider2D _col;
    private Camera _cam;
    private Chip lastEquippedChip; // local cached ref used by Unequip()

    private void Start()
    {
        if (transform.parent != null)
            turretInventory = transform.parent.GetComponent<TurretInventory>();

        _col = GetComponent<Collider2D>();

        if (saveLoader == null)
            saveLoader = Object.FindAnyObjectByType<SaveLoader>();
    }

    private void Update()
    {
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame) return;
        if (inventory == null || inventory.ChipSelected == null) return;

        if (_cam == null) _cam = CameraCache.Main;
        if (_cam == null) return;

        Vector2 worldPos = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Collider2D hit = Physics2D.OverlapPoint(worldPos, clickableMask);
        if (hit == _col) Equip();
    }

    public void Equip()
    {
        if (inventory == null || inventory.ChipSelected == null) return;

        var chip = inventory.ChipSelected;
        ApplyEquip(chip);
        TriggerSave();
    }

    public void LoadEquip(Chip chip)
    {
        if (chip == null) return;

        // Save-load path: stage the chip as "selected" so ApplyEquip's
        // shared logic still works, then reset the selection at the end.
        inventory.ChipSelected = chip;
        ApplyEquip(chip);
        TriggerSave();
    }

    public void Unequip()
    {
        if (occupiedChip == null) return;

        var chip = occupiedChip;
        inventory.Collect(chip);
        if (turretInventory != null) turretInventory.Unapply(chip);

        chip.equippedSlot = null;
        occupiedChip = null;
        if (inventory != null) inventory.ChipSelected = null;

        GameEvents.RaiseChipUnequipped(chip, this);
    }

    private void ApplyEquip(Chip chip)
    {
        if (chip == null) return;

        if (inventory != null) inventory.Drop(chip);

        var t = chip.transform;
        t.SetParent(transform);
        t.localPosition = Vector3.zero;

        occupiedChip = chip;
        lastEquippedChip = chip;

        if (turretInventory != null) turretInventory.Apply(chip);

        if (inventory != null) inventory.ChipSelected = null;
        chip.equippedSlot = this;
        chip.Unselect();

        GameEvents.RaiseChipEquipped(chip, this);
    }

    private void TriggerSave()
    {
        if (saveLoader != null) saveLoader.Save();
        else GameEvents.RaiseSaveRequested();
    }
}
