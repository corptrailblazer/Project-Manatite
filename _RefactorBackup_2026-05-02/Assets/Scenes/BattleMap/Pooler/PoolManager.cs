using UnityEngine;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour
{
    // Optional: drag common prefabs + counts to prewarm in Awake
    [System.Serializable] public struct PrewarmEntry { public GameObject prefab; public int count; }
    [SerializeField] private List<PrewarmEntry> prewarm = new();

    private readonly Dictionary<GameObject, Queue<GameObject>> pools = new();

    void Awake()
    {
        foreach (var e in prewarm)
            Prewarm(e.prefab, Mathf.Max(0, e.count));
    }

    public void Prewarm(GameObject prefab, int count)
    {
        if (!pools.TryGetValue(prefab, out var q))
            pools[prefab] = q = new Queue<GameObject>();

        for (int i = 0; i < count; i++)
        {
            var go = CreateInstance(prefab);
            go.SetActive(false);
            q.Enqueue(go);
        }
    }

    GameObject CreateInstance(GameObject prefab)
    {
        var go = Instantiate(prefab, transform);
        var pi = go.GetComponent<PoolItem>() ?? go.AddComponent<PoolItem>();
        pi.prefabKey = prefab;
        pi.manager = this;
        return go;
    }

    public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent = null)
    {
        if (!pools.TryGetValue(prefab, out var q))
            pools[prefab] = q = new Queue<GameObject>();

        GameObject go = q.Count > 0 ? q.Dequeue() : CreateInstance(prefab);
        go.transform.SetPositionAndRotation(pos, rot);
        if (parent) go.transform.SetParent(parent, false);
        go.SetActive(true);

        if (go.TryGetComponent<IPoolable>(out var p)) p.OnSpawned();
        return go;
    }

    public void Despawn(GameObject go)
    {
        if (!go) return;
        if (go.TryGetComponent<IPoolable>(out var p)) p.OnDespawned();

        var pi = go.GetComponent<PoolItem>();
        if (pi == null || pi.prefabKey == null) { Destroy(go); return; } // fallback if not pooled

        go.SetActive(false);
        go.transform.SetParent(transform, false);
        if (!pools.TryGetValue(pi.prefabKey, out var q))
            pools[pi.prefabKey] = q = new Queue<GameObject>();
        q.Enqueue(go);
    }
}
