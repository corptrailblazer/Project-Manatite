using ProjectManatite.Core;
using UnityEngine;

/// <summary>
/// Component placed on the explosion VFX prefab. Damages any enemy that overlaps
/// the explosion's trigger collider during its lifetime.
///
/// Refactor notes vs. original: routes damage through <see cref="IDamageable"/>
/// so the explosion can also damage destructibles in future without changes here.
/// </summary>
public class ExplosionModifier : MonoBehaviour
{
    [SerializeField] private ExplosionModifierSO config;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (config == null) return;
        if (!other.CompareTag("Enemy")) return;

        if (other.TryGetComponent<IDamageable>(out var damageable))
            damageable.TakeDamage(config.damage, gameObject);
    }
}
