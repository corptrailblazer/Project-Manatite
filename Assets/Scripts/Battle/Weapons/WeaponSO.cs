using UnityEngine;

/// <summary>
/// Designer-authored stat block for a weapon archetype.
/// Drop into the inspector field on <see cref="WeaponBehaviour"/>; chip modifiers
/// stack on top of these base values at runtime.
/// </summary>
[CreateAssetMenu(menuName = "Weapons/Weapon", fileName = "WeaponSO")]
public class WeaponSO : ScriptableObject
{
    [Tooltip("Stable identifier used by save data and analytics.")]
    public string id;

    [Tooltip("Prefab containing the WeaponBehaviour that consumes this SO.")]
    public GameObject prefab;

    [Header("Base stats")]
    [Min(0)] public float damage = 10;

    [Tooltip("Shots per second.")]
    [Min(0)] public float fireRate = 4;

    [Min(0)] public float projSpeed = 12;
}
