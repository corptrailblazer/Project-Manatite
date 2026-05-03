using UnityEngine;

/// <summary>
/// Designer-authored stat block for a chip. Plugged into a <see cref="Chip"/>
/// MonoBehaviour, then read by <see cref="WeaponBehaviour"/> when applied to a turret.
///
/// <para>Field names are kept identical to the original asset YAML so existing
/// chip <c>.asset</c> files deserialize unchanged.</para>
/// </summary>
[CreateAssetMenu(fileName = "Chips", menuName = "Chips/BaseChip")]
public class ChipSO : ScriptableObject
{
    [Header("Modifiers")]
    [Tooltip("Multiplicative damage bonus. 0.5 = +50%, -0.5 = -50%.")]
    [SerializeField, Range(-5f, 5f)] public float damagePct;

    [Tooltip("Flat damage added before the multiplier.")]
    [SerializeField] public float damageFlat;

    [Tooltip("Multiplicative fire-rate bonus. 0.5 = +50% shots/sec.")]
    [SerializeField, Range(-5f, 5f)] public float fireRatePct;

    [SerializeField] public float fireRateFlat;

    [Tooltip("Multiplicative projectile-speed bonus.")]
    [SerializeField, Range(-5f, 5f)] public float projSpeedPct;

    [SerializeField] public float projSpeedFlat;

    [Tooltip("Optional projectile side-effect (e.g. explosion on hit).")]
    [SerializeField] public ModifierInterface modifier;

    [Header("Display")]
    [SerializeField] public string chipName;
    [SerializeField] public string effect;
    [SerializeField] public Sprite sprite;

    [Header("Drop")]
    [Tooltip("Probability that this chip drops when an enemy with this in its drop table dies.")]
    [Range(0f, 1f)] public float dropRate;
}
