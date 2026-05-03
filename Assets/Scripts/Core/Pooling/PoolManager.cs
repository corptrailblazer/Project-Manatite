using System.Collections.Generic;
using UnityEngine;

namespace ProjectManatite.Core
{
    /// <summary>
    /// Generic prefab object pool. Each prefab gets its own dictionary-keyed queue.
    /// <para>Spawn / Despawn are O(1) amortised. <see cref="IPoolable"/> hooks fire on
    /// activation and deactivation so consumers can reset per-instance state.</para>
    ///
    /// <para>Drop common prefabs into <see cref="_prewarm"/> in the inspector to
    /// pre-instantiate them at <see cref="Awake"/> and avoid hitches mid-gameplay.</para>
    /// </summary>
    public sealed class PoolManager : MonoBehaviour
    {
        [System.Serializable]
        public struct PrewarmEntry
        {
            public GameObject Prefab;
            [Min(0)] public int Count;
        }

        [Tooltip("Prefabs to pre-instantiate on Awake. Reduces frame-spike when an enemy spawns / projectile fires for the first time.")]
        [SerializeField] private List<PrewarmEntry> _prewarm = new();

        private readonly Dictionary<GameObject, Queue<GameObject>> _pools = new();

        private void Awake()
        {
            for (int i = 0; i < _prewarm.Count; i++)
            {
                var entry = _prewarm[i];
                if (entry.Prefab == null) continue;
                Prewarm(entry.Prefab, Mathf.Max(0, entry.Count));
            }
        }

        /// <summary>
        /// Pre-instantiate <paramref name="count"/> instances of <paramref name="prefab"/>
        /// and park them in the pool, deactivated.
        /// </summary>
        public void Prewarm(GameObject prefab, int count)
        {
            if (prefab == null) return;
            var queue = GetOrCreateQueue(prefab);
            for (int i = 0; i < count; i++)
            {
                var instance = CreateInstance(prefab);
                instance.SetActive(false);
                queue.Enqueue(instance);
            }
        }

        /// <summary>
        /// Activate (or instantiate) a <paramref name="prefab"/> instance at the given
        /// pose. Reparents under <paramref name="parent"/> if provided, otherwise under
        /// this manager.
        /// </summary>
        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (prefab == null) return null;

            var queue = GetOrCreateQueue(prefab);
            GameObject go = queue.Count > 0 ? queue.Dequeue() : CreateInstance(prefab);

            // Skip-frame proof: an instance returned to the pool can sometimes be
            // destroyed externally; treat that as "miss" and create a new one.
            if (go == null)
            {
                go = CreateInstance(prefab);
            }

            go.transform.SetPositionAndRotation(position, rotation);
            if (parent != null) go.transform.SetParent(parent, false);
            go.SetActive(true);

            if (go.TryGetComponent<IPoolable>(out var poolable))
                poolable.OnSpawned();

            return go;
        }

        /// <summary>Return an instance to its pool. Safe to call on null or non-pooled objects (those are destroyed).</summary>
        public void Despawn(GameObject go)
        {
            if (go == null) return;

            if (go.TryGetComponent<IPoolable>(out var poolable))
                poolable.OnDespawned();

            // Non-pooled fallback: just destroy.
            if (!go.TryGetComponent<PoolItem>(out var item) || item.PrefabKey == null)
            {
                Destroy(go);
                return;
            }

            go.SetActive(false);
            go.transform.SetParent(transform, false);
            GetOrCreateQueue(item.PrefabKey).Enqueue(go);
        }

        private GameObject CreateInstance(GameObject prefab)
        {
            var go = Instantiate(prefab, transform);
            if (!go.TryGetComponent<PoolItem>(out var item))
                item = go.AddComponent<PoolItem>();
            item.PrefabKey = prefab;
            item.Manager = this;
            return go;
        }

        private Queue<GameObject> GetOrCreateQueue(GameObject prefab)
        {
            if (!_pools.TryGetValue(prefab, out var queue))
                _pools[prefab] = queue = new Queue<GameObject>();
            return queue;
        }
    }
}
