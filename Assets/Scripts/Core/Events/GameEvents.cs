using System;
using UnityEngine;

namespace ProjectManatite.Core
{
    /// <summary>
    /// Static event bus for cross-system communication.
    ///
    /// Lets producers (Enemy, Player, Chip) raise events without knowing about consumers
    /// (UI, Save system, WinLevel). Replaces the FindAnyObjectByType / GameObject.Find
    /// patterns scattered through the original code.
    ///
    /// Subscribe in OnEnable, unsubscribe in OnDisable. Events automatically clear
    /// when entering Play mode (see <see cref="ResetOnDomainReload"/>) so editor-only
    /// stale handlers don't leak.
    /// </summary>
    public static class GameEvents
    {
        // ---------- Enemy ----------
        /// <summary>Raised after an enemy fully spawns (after pool init).</summary>
        public static event Action<GameObject> EnemySpawned;
        /// <summary>Raised when an enemy reaches 0 HP (before despawn).</summary>
        public static event Action<GameObject> EnemyDied;

        // ---------- Player ----------
        /// <summary>Raised whenever the player's HP changes. Args: (current, max).</summary>
        public static event Action<float, float> PlayerHealthChanged;
        /// <summary>Raised exactly once on player death.</summary>
        public static event Action PlayerDied;

        // ---------- Economy ----------
        /// <summary>Raised when coins are earned or spent. Arg: signed delta.</summary>
        public static event Action<int> CoinsChanged;

        // ---------- Chips / Inventory ----------
        /// <summary>Chip dropped into the world by an enemy.</summary>
        public static event Action<Chip> ChipDropped;
        /// <summary>Chip picked up into an inventory slot.</summary>
        public static event Action<Chip> ChipCollected;
        /// <summary>Chip selected (highlighted) in inventory.</summary>
        public static event Action<Chip> ChipSelected;
        /// <summary>Chip equipped onto a turret/weapon.</summary>
        public static event Action<Chip, SlotBehaviour> ChipEquipped;
        /// <summary>Chip removed from a turret/weapon back into inventory.</summary>
        public static event Action<Chip, SlotBehaviour> ChipUnequipped;

        // ---------- Save / Load ----------
        /// <summary>Fires before serialization so systems can flush state.</summary>
        public static event Action SaveRequested;
        /// <summary>Fires after deserialization so systems can apply state.</summary>
        public static event Action LoadCompleted;

        // ---------- Battle outcome ----------
        public static event Action BattleWon;
        public static event Action BattleLost;

        // ---------- Raise helpers (null-safe) ----------
        public static void RaiseEnemySpawned(GameObject go)              => EnemySpawned?.Invoke(go);
        public static void RaiseEnemyDied(GameObject go)                 => EnemyDied?.Invoke(go);
        public static void RaisePlayerHealthChanged(float cur, float max) => PlayerHealthChanged?.Invoke(cur, max);
        public static void RaisePlayerDied()                             => PlayerDied?.Invoke();
        public static void RaiseCoinsChanged(int delta)                  => CoinsChanged?.Invoke(delta);
        public static void RaiseChipDropped(Chip c)                      => ChipDropped?.Invoke(c);
        public static void RaiseChipCollected(Chip c)                    => ChipCollected?.Invoke(c);
        public static void RaiseChipSelected(Chip c)                     => ChipSelected?.Invoke(c);
        public static void RaiseChipEquipped(Chip c, SlotBehaviour s)    => ChipEquipped?.Invoke(c, s);
        public static void RaiseChipUnequipped(Chip c, SlotBehaviour s)  => ChipUnequipped?.Invoke(c, s);
        public static void RaiseSaveRequested()                          => SaveRequested?.Invoke();
        public static void RaiseLoadCompleted()                          => LoadCompleted?.Invoke();
        public static void RaiseBattleWon()                              => BattleWon?.Invoke();
        public static void RaiseBattleLost()                             => BattleLost?.Invoke();

        /// <summary>
        /// Clears every subscriber list when entering play mode. Without this, the
        /// editor's domain-reload optimization (Project Settings &gt; Editor &gt; Enter Play
        /// Mode Settings) would keep stale handlers from previous play sessions.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOnDomainReload()
        {
            EnemySpawned = null;
            EnemyDied = null;
            PlayerHealthChanged = null;
            PlayerDied = null;
            CoinsChanged = null;
            ChipDropped = null;
            ChipCollected = null;
            ChipSelected = null;
            ChipEquipped = null;
            ChipUnequipped = null;
            SaveRequested = null;
            LoadCompleted = null;
            BattleWon = null;
            BattleLost = null;
        }
    }
}
