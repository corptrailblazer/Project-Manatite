using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    [Tooltip("Local position (relative to this Inventory transform)")]
    public Vector3 targetLocalPosition;
    [SerializeField] private GameObject invprefab;
    [SerializeField] private Transform inv;

    [SerializeField] int slotspacing = 2;
    private List<InventorySlot> slots = new List<InventorySlot>();
    private GameObject selectedSlot = null;
    public Chip ChipSelected = null;

    void Start()
    {
        var sr = GetComponent<SpriteRenderer>();
        var prefabsr = invprefab.GetComponent<SpriteRenderer>();

        Vector3 bounds = prefabsr.sprite.bounds.size;
        Vector3 scale = prefabsr.transform.lossyScale;

        Vector2 prefabWorldSize = new Vector2(bounds.x * scale.x, bounds.y * scale.y);
        float spacing = prefabWorldSize.x + slotspacing;

        for (float i = 0; i + spacing < sr.size.x; i += spacing)
        {
            var go = Instantiate(invprefab, inv);
            go.transform.localPosition = targetLocalPosition;
            targetLocalPosition.x += spacing;
            slots.Add(go.GetComponent<InventorySlot>());
        }
        
    }

    public void Collect(Chip chip)
    {
        if (chip == null) return;

        var t   = chip.transform;
        var rb  = chip.GetComponent<Rigidbody2D>();
        var col = chip.GetComponent<Collider2D>();

        foreach (var invslot in slots)
        {
            if(!invslot.occupied)
            {
                selectedSlot = invslot.gameObject;
                invslot.occupied = true;
                invslot.storedChip = chip;
                break;
            }
        }

        if (rb)
        { 
            rb.linearVelocity = Vector2.zero; 
            rb.angularVelocity = 0f; 
        }

        t.SetParent(transform, worldPositionStays: false);
        t.localPosition = selectedSlot.transform.localPosition;
        t.localRotation = Quaternion.identity;

    }

    public void Drop(Chip chip)
    {
        if (chip == null) return;

        foreach (var invslot in slots)
        {
            if(invslot.storedChip == chip)
            {
                invslot.occupied = false;
                invslot.storedChip = null;
                break;
            }
        }

        /*var t   = chip.transform;
        var rb  = chip.GetComponent<Rigidbody2D>();
        var col = chip.GetComponent<Collider2D>();

        t.SetParent(null, worldPositionStays: true);
        t.position = worldPos;

        if (rb) rb.simulated = true;
        if (col) col.enabled = true;*/
    }
}