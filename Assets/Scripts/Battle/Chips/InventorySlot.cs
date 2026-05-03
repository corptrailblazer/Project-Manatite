using UnityEngine;

/// <summary>
/// Data holder attached to each generated inventory slot. Tracks whether the slot
/// currently holds a <see cref="Chip"/>.
///
/// Empty <c>Start</c> / <c>Update</c> from the original have been removed.
/// </summary>
[DisallowMultipleComponent]
public class InventorySlot : MonoBehaviour
{
    [Tooltip("0-based index into Inventory.slots.")]
    public int index;

    public bool occupied;
    public Chip storedChip;
}
