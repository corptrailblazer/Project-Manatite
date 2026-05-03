using UnityEngine;
using UnityEngine.InputSystem;

public class SlotBehaviour : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private TurretInventory turretInventory;
    public Chip occupiedChip = null;
    [SerializeField] LayerMask clickableMask = ~0;
    [SerializeField] Inventory inventory;
    Collider2D col;
    Chip isChip;
    SaveLoader saveloader;

    void Start()
    {
        turretInventory = transform.parent.gameObject.GetComponent<TurretInventory>();
        col = GetComponent<Collider2D>();
        saveloader = Object.FindAnyObjectByType<SaveLoader>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        Collider2D hit = Physics2D.OverlapPoint(worldPos, clickableMask);

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (hit != null && hit == col)
            {
                if (inventory.ChipSelected != null)
                {
                    Equip();
                }
            }
        }
    }

    /*private void OnTriggerStay2D(Collider2D other)
    {
        
        Chip isChip = other.GetComponent<Chip>();
        Debug.Log(isChip.isInventory);
        if (other.gameObject.tag == "Ship"&& other.gameObject.tag == "Enemy") 
            return;
        if (isChip != null && occupiedChip == null && isChip.isInventory) // só conta se foi solto
        {
            isChip.isEquiped = true;
            if(!Mouse.current.leftButton.isPressed)
            {
                other.gameObject.transform.SetParent(this.transform);
                other.gameObject.transform.localPosition = Vector3.zero;
                occupiedChip = isChip;
                turretInventory.Apply(isChip);
            }
            
        }
        else if(isChip != null)
        {
            isChip.isEquiped = false;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Chip isChip = other.GetComponent<Chip>();
        if (isChip != null && occupiedChip == isChip)
        {
            occupiedChip = null;
            isChip.isEquiped = false;
            turretInventory.Unapply(isChip);
        }
            
            
    }
    */

    public void Equip()
    {
         
        isChip = inventory.ChipSelected.GetComponent<Chip>();

        inventory.Drop(isChip);
        inventory.ChipSelected.gameObject.transform.SetParent(this.transform);
        inventory.ChipSelected.gameObject.transform.localPosition = Vector3.zero;
        occupiedChip = isChip;
        turretInventory.Apply(isChip);
        inventory.ChipSelected = null;
        isChip.equippedSlot = this;
        isChip.Unselect();
        saveloader.Save();
        
    }

    public void LoadEquip(Chip isChip)
    {
         
        Debug.Log("TESTEEE");

        inventory.Drop(isChip);
        inventory.ChipSelected = isChip;
        inventory.ChipSelected.gameObject.transform.SetParent(this.transform);
        inventory.ChipSelected.gameObject.transform.localPosition = Vector3.zero;
        occupiedChip = isChip;
        turretInventory.Apply(isChip);
        inventory.ChipSelected = null;
        isChip.equippedSlot = this;
        isChip.Unselect();
        saveloader.Save();
        
    }

    public void Unequip()
    {
        inventory.Collect(occupiedChip);
        turretInventory.Unapply(isChip);

        occupiedChip = null;
        inventory.ChipSelected = null;
        isChip.equippedSlot = null;
    }

}
