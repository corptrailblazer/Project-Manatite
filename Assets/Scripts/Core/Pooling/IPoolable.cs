namespace ProjectManatite.Core
{
    /// <summary>
    /// Optional hook for components that need to react when their host
    /// <see cref="UnityEngine.GameObject"/> is taken from / returned to a pool.
    ///
    /// <para>Called by <see cref="PoolManager"/> after Spawn() activates the object,
    /// and before Despawn() deactivates it. Use this to reset state (HP, velocity,
    /// timers) so a recycled instance behaves like a fresh one.</para>
    /// </summary>
    public interface IPoolable
    {
        void OnSpawned();
        void OnDespawned();
    }
}
