using UnityEngine;

[CreateAssetMenu(menuName="Weapons/Slot Modifier")]
public class SlotModifierSO : ScriptableObject
{


    [Header("Multiplicadores (1 = neutro)")]
    public float damageMul   = 1f;
    public float fireRateMul = 1f;
    public float projSpeedMul= 1f;

    [Header("Adições")]
    public float damageAdd   = 0f;
    public float fireRateAdd = 0f;
    public float projSpeedAdd= 0f;
}
