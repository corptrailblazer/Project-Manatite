using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Chip : MonoBehaviour
{

    [Header("Falling Speed")]
    public float fall_speed = 1f;

    [SerializeField] Inventory inventory;
    [SerializeField] LayerMask clickableMask = ~0;

    public Collider2D col;
    Rigidbody2D rb;
    //bool isDragging = false;
    public bool isInventory;
    bool isHovering;
    //bool isPosReseted;
    public SlotBehaviour equippedSlot = null;
    [SerializeField] Tooltip tooltipPrefab;
    public ChipSO chipSO;
    public bool isSelected;
    SpriteRenderer sr;
    Tooltip tooltip = null;
    SaveLoader saveloader;

    

    void Awake()
    {

        sr = this.GetComponent<SpriteRenderer>();
        if (inventory == null)
            inventory = Object.FindAnyObjectByType<Inventory>();
        saveloader = Object.FindAnyObjectByType<SaveLoader>();

    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(0, fall_speed * -1);
        
        col = GetComponent<Collider2D>();

        StartCoroutine(autoDestroy());

    }


    void Update()
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        Collider2D hit = Physics2D.OverlapPoint(worldPos, clickableMask);

        if (equippedSlot == null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame && !isInventory)
                Collect(hit);

            if (Mouse.current.leftButton.wasPressedThisFrame && isInventory)
                Select(hit);
        }

        else
        {
            if (Mouse.current.leftButton.wasPressedThisFrame && hit != null && hit == col)
                equippedSlot.Unequip();
        }

        isHovering = (col == hit);

        if(isHovering)
            StartHover();

        if(!isHovering)
            StopHover();
        

        


        /*
        if(isHovering && !isDragging)
            StartHover();

        if(!isHovering && !isPosReseted)
            StopHover();

        if (!isDragging && Mouse.current.leftButton.wasPressedThisFrame && isInventory)
            StartDrag(hit);

        if (isDragging && Mouse.current.leftButton.isPressed)
            MoveAlongCursor(worldPos);

        if (isDragging && Mouse.current.leftButton.wasReleasedThisFrame)
            Release(worldPos);
        */

        
            
    }

    void Collect(Collider2D hit)
    {
        if (hit != null && hit == col)
        {
            inventory.Collect(this);
            //chipSO.dropRate /= 10;
            StartCoroutine(setInventory());
        }
    }

    public void Collect()
    {
        inventory.Collect(this);
        StartCoroutine(setInventory());
    }

    void Select(Collider2D hit)
    {
        if (hit != null && hit == col)
        {
            if (!isSelected)
            {
                isSelected = true;
                inventory.ChipSelected = this;
                sr.color = new Color(.5f, .5f, .5f, 1f);
            }
             
            else
            {
                Unselect();
            }

        }
    }

    public void Unselect()
    {
        isSelected = false;
        inventory.ChipSelected = null;
        sr.color = new Color(1f, 1f, 1f, 1f);
    }

    /*void StartDrag(Collider2D hit)
    {
        if (hit != null && hit == col)
        isDragging = true;
    }

    void MoveAlongCursor(Vector2 worldPos)
    {
        transform.position = worldPos;
    }

    void Release(Vector2 worldPos)
    {
        inventory.Drop(this, worldPos);
        isInventory = false;
        isDragging = false;

        if(!isEquiped)
        {
            inventory.Collect(this);
            StartCoroutine(setInventory());
        }
        
    }*/

    void StartHover()
    {
        if (tooltip == null)
        {
            tooltip = Instantiate(tooltipPrefab, this.transform);
            tooltip.GetComponent<Tooltip>().updateInfo(this);
        }
            
        tooltip.transform.localPosition = Vector3.zero;
        tooltip.transform.localRotation = Quaternion.identity;

        tooltip.transform.localPosition = tooltip.toolPlacement;
        
    }

    void StopHover()
    {
        
        if(gameObject.transform.childCount > 0) 
        {
            Destroy(tooltip.gameObject);
        }
            
        
    }

    IEnumerator setInventory()
    {
        yield return new WaitForSeconds(0.1f);
        isInventory = true;
        yield return new WaitForSeconds(1f);
        saveloader.Save();
    }

    IEnumerator setEquipped()
    {
        yield return new WaitForSeconds(0.3f);
        //equippedSlot = this;
    }

    public IEnumerator autoDestroy()
    {
        yield return new WaitForSeconds(30f);
        if(!isInventory)
            Destroy(gameObject);
    }

}
