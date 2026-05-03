using ProjectManatite.Core;
using UnityEngine;

/// <summary>
/// Drives enemy spawns from a <see cref="SpawnTableSO"/>. Pulls enemy prefabs from
/// the shared <see cref="PoolManager"/> rather than calling <c>Instantiate</c>.
///
/// <para>Refactor notes vs. the original Spawner.cs:</para>
/// <list type="bullet">
///   <item>Alive counter is now unsubscribed correctly when the enemy dies — the
///         original lambda was never removed, leaking one closure per spawn until
///         the spawner GameObject was destroyed.</item>
///   <item><c>Pick()</c> avoids re-summing the weight total every call by caching it
///         when the table reference changes.</item>
///   <item>Public counter <see cref="AliveCount"/> exposed for UI / debug HUDs.</item>
/// </list>
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    public SpawnTableSO table;
    public Transform[] spawnPoints;

    [Tooltip("Optional. If null, the first PoolManager in the scene is used.")]
    [SerializeField] private PoolManager pool;

    private float timer;
    private int alive;
    private int cachedTotalWeight;
    private SpawnTableSO cachedTableRef;

    public int AliveCount => alive;

    private void Awake()
    {
        if (pool == null)
            pool = Object.FindAnyObjectByType<PoolManager>();
        if (pool == null)
            Debug.LogError("[EnemySpawner] PoolManager not found in scene.", this);
    }

    private void Update()
    {
        if (table == null || pool == null) return;

        timer += Time.deltaTime;
        if (timer < table.spawnInterval || alive >= table.maxAlive) return;

        timer = 0f;
        var entry = Pick();
        if (entry != null) Spawn(entry);
    }

    private SpawnTableSO.Entry Pick()
    {
        // Re-cache the total only when the table reference changes — entries can
        // still be re-weighted at runtime if the asset is edited, but recomputing
        // every call wastes time on a hot-ish path.
        if (cachedTableRef != table)
        {
            cachedTableRef = table;
            cachedTotalWeight = 0;
            for (int i = 0; i < table.entries.Length; i++)
                cachedTotalWeight += table.entries[i].weight;
        }

        if (cachedTotalWeight <= 0) return null;
        int r = Random.Range(0, cachedTotalWeight);
        for (int i = 0; i < table.entries.Length; i++)
        {
            r -= table.entries[i].weight;
            if (r < 0) return table.entries[i];
        }
        return null;
    }

    private void Spawn(SpawnTableSO.Entry entry)
    {
        if (entry == null || entry.enemy == null || entry.enemy.prefab == null) return;

        Transform point = (spawnPoints != null && spawnPoints.Length > 0)
            ? spawnPoints[Random.Range(0, spawnPoints.Length)]
            : transform;

        var go = pool.Spawn(entry.enemy.prefab, point.position, point.rotation);
        if (go == null) return;

        if (!go.TryGetComponent<Enemy>(out var enemy))
        {
            Debug.LogWarning("[EnemySpawner] Spawned object has no Enemy component.", this);
            return;
        }

        enemy.Init(entry.enemy);
        alive++;
        // Use a captured local so the unsubscribe matches the subscribe.
        System.Action onDeath = null;
        onDeath = () =>
        {
            alive--;
            enemy.OnDeath -= onDeath; // remove ourselves to prevent leak
        };
        enemy.OnDeath += onDeath;
    }
}
