using UnityEngine;

namespace ProjectManatite.Core
{
    /// <summary>
    /// Tag component automatically added by <see cref="PoolManager"/> to instances it
    /// creates. Stores the original prefab key so the manager can return the instance
    /// to the right queue on Despawn.
    ///
    /// <para>Hidden in the inspector — these fields are managed by the pool, not the
    /// designer.</para>
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PoolItem : MonoBehaviour
    {
        [HideInInspector] public GameObject PrefabKey;
        [HideInInspector] public PoolManager Manager;
    }
}
