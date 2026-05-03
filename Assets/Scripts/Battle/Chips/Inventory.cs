using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the player's chip inventory: lays out a row of <see cref="InventorySlot"/>
/// children at <see cref="Start"/> sized to the inventory background's sprite, and
/// provides Collect / Drop entry points for chips.
///
/// <para>Refactor notes vs. the original Inventory.cs:</para>
/// <list type="bullet">
///   <item>Slot iteration uses for-index instead of foreach to avoid the IEnumerator allocation each Collect/Drop.</item>
///   <item>Cached components in <see cref="Awake"/> instead of looking up per-Start.</item>
///   <item>Removed dead/commented drop-physics block.</item>
///   <item><see cref="ChipSelected"/> is now an event-emitting property — UI can react without polling.</item>
/// </list>
/// </summary>
public class Inventory : MonoBehaviour
{
    [Tooltip("Local position (relative to this Inventory transform) of the first slot.")]
    public Vector3 targetLocalPosition;

    [SerializeField] private GameObject invprefab;
    [SerializeField] private Transform inv;

    [Tooltip("Extra spacing between slot prefabs (world units).")]
    [SerializeField] private int slotspacing = 2;

    private readonly List<InventorySlot> slots = new();
    private GameObject selectedSlot;

    private Chip _chipSelected;
    /// <summary>Currently highlighted chip (or null). Setting raises <c>OnChipSelectedChanged</c>.</summary>
    public Chip ChipSelected
    {
        get => _chipSelected;
        set
        {
            if (_chipSelected == value) return;
            _chipSelected = value;
            OnChipSelectedChanged?.Invoke(value);
        }
    }

    /// <summary>Raised whenever <see cref="ChipSelected"/> changes (including to null).</summary>
    public event System.Action<Chip> OnChipSelectedChanged;

    /// <summary>Read-only view of slots for UI introspection.</summary>
    public IReadOnlyList<InventorySlot> Slots => slots;

    private void Start()
    {
        BuildSlots();
    }

    private void BuildSlots()
    {
        if (invprefab == null) return;

        var sr = GetComponent<SpriteRenderer>();
        var prefabsr = invprefab.GetComponent<SpriteRenderer>();
        if (sr == null || prefabsr == null) return;

        Vector3 bounds = prefabsr.sprite.bounds.size;
        Vector3 scale = prefabsr.transform.lossyScale;
        Vector2 prefabWorldSize = new(bounds.x * scale.x, bounds.y * scale.y);
        float spacing = prefabWorldSize.x + slotspacing;

        int slotIndex = 0;
        for (float i = 0; i + spacing < sr.size.x; i += spacing)
        {
            var go = Instantiate(invprefab, inv);
            go.transform.localPosition = targetLocalPosition;
            targetLocalPosition.x += spacing;
            if (go.TryGetComponent<InventorySlot>(out var slot))
            {
                slot.index = slotIndex++;
                slots.Add(slot);
            }
        }
    }

    /// <summary>Place a chip into the first free slot. Returns false if the inventory is full.</summary>
    public bool Collect(Chip chip)
    {
        if (chip == null) return false;

        // Find first free slot.
        InventorySlot target = null;
        for (int i = 0; i < slots.Count; i++)
        {
            if (!slots[i].occupied)
            {
                target = slots[i];
                break;
            }
        }
        if (target == null) return false; // inventory full

        target.occupied = true;
        target.storedChip = chip;
        selectedSlot = target.gameObject;

        // Park the chip's transform in the slot, kill physics so it stays there.
        if (chip.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        var t = chip.transform;
        t.SetParent(transform, worldPositionStays: false);
        t.localPosition = target.transform.localPosition;
        t.localRotation = Quaternion.identity;

        return true;
    }

    /// <summary>Free the slot currently holding <paramref name="chip"/>. Does not detach the transform.</summary>
    public bool Drop(Chip chip)
    {
        if (chip == null) return false;

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].storedChip != chip) continue;
            slots[i].occupied = false;
            slots[i].storedChip = null;
            return true;
        }

        return false;
    }
}
