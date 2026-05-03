/*
using UnityEngine;
using System.Linq;

public enum SlotPosition { L, F, R} 
public class WeaponMount : MonoBehaviour
{

    public SlotPosition SlotPosition = SlotPosition.L;
    public SlotType slotType = SlotType.Generic;
    public SlotModifierSO[] slotMods; // os “traits” específicos desse slot
    public Transform firePoint;       // onde nasce o projétil

    WeaponBehaviour current;

    public bool TryEquip(WeaponSO weapon)
    {
        if (weapon == null) return false;
        if (weapon.allowedSlots.Length > 0 && !weapon.allowedSlots.Contains(slotType))
            return false;

        if (current != null) Destroy(current.gameObject);

        // Instancia o prefab da arma nesse slot
        Vector3 parentScale = transform.lossyScale;
        Quaternion rotation = transform.rotation;
        
        switch(SlotPosition) 
        {
        case SlotPosition.L:
            rotation = Quaternion.Euler(0f, 0f, 270f);
            firePoint.rotation = Quaternion.Euler(0f, 0f, -270f);;
            break;
        case SlotPosition.R:
            rotation = Quaternion.Euler(0f, 180f, 270f);
            firePoint.rotation = Quaternion.Euler(0f, 180f, -270f);;
            break;
        default:
            break;
        }

        var go = Instantiate(weapon.prefab, transform.position, rotation, transform);
        current = go.GetComponent<WeaponBehaviour>();
        go.transform.localScale = new Vector2(1f/ parentScale.x, 1f / parentScale.y);


        // Calcula stats finais = base -> multiplica -> soma
        float dmg   = weapon.damage;
        float rate  = weapon.fireRate;
        float speed = weapon.projSpeed;

        foreach (var m in slotMods)
        {
            dmg   = dmg   * m.damageMul   + m.damageAdd;
            rate  = rate  * m.fireRateMul + m.fireRateAdd;
            speed = speed * m.projSpeedMul+ m.projSpeedAdd;
        }

        //current.Init(this, dmg, rate, speed); // passa stats e firePoint via mount
        return true;
    }

    public Transform GetFirePoint() => firePoint != null ? firePoint : transform;
}
*/