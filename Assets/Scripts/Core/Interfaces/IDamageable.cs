using System;
using UnityEngine;

namespace ProjectManatite.Core
{
    /// <summary>
    /// Anything that can take damage. Decouples damage producers (projectiles,
    /// explosions) from damage consumers (enemies, the player, destructible props).
    /// </summary>
    public interface IDamageable
    {
        /// <summary>Current health, clamped to <see cref="MaxHealth"/>.</summary>
        float CurrentHealth { get; }

        /// <summary>Maximum health configured for this entity.</summary>
        float MaxHealth { get; }

        /// <summary>True if <see cref="CurrentHealth"/> &lt;= 0.</summary>
        bool IsDead { get; }

        /// <summary>Apply <paramref name="amount"/> damage from <paramref name="source"/> (source is optional / for analytics, knockback, etc.).</summary>
        void TakeDamage(float amount, GameObject source = null);

        /// <summary>Restore <paramref name="amount"/> health, clamped at <see cref="MaxHealth"/>.</summary>
        void Heal(float amount);

        /// <summary>Raised whenever current health changes. Args: (current, max).</summary>
        event Action<float, float> OnHealthChanged;

        /// <summary>Raised exactly once when CurrentHealth crosses zero.</summary>
        event Action OnDied;
    }
}
