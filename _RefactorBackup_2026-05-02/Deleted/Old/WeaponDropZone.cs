/*

using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Collider2D))]
public class WeaponDropZone : MonoBehaviour
{
    public WeaponMount mount; // referência ao próprio mount


    private void OnTriggerStay2D(Collider2D other)
    {
        // Checa se o objeto arrastado tem DraggableWeapon
        DraggableWeapon draggable = other.GetComponent<DraggableWeapon>();
        if (draggable != null && !Mouse.current.leftButton.isPressed) // só conta se foi solto
        {
            WeaponSO weapon = draggable.weaponData;
            if (weapon != null)
            {    
                
                bool equiped = mount.TryEquip(weapon);
                if(equiped) Destroy(other.gameObject);
                else draggable.transform.position = draggable.startPosition;
                 
            }
        }
    }
}
*/