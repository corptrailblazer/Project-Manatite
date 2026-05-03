using UnityEngine;

[CreateAssetMenu(menuName="Weapons/Weapon")]
public class WeaponSO : ScriptableObject
{
    public string id;
    public GameObject prefab;        // prefab com WeaponBehaviour

    [Header("Stats base")]
    public float damage = 10;
    public float fireRate = 4;       // tiros/seg
    public float projSpeed = 12;
}
