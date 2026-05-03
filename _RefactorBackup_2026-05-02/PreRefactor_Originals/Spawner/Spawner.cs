using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public SpawnTableSO table;
    public Transform[] spawnPoints;

    [SerializeField] private PoolManager pool; // assign or auto-find

    float timer;
    int alive;

    void Awake()
    {
        if (pool == null)
            pool = Object.FindAnyObjectByType<PoolManager>();
        if (pool == null)
            Debug.LogError("[EnemySpawner] PoolManager not found in scene.");
    }

    void Update()
    {
        if (table == null || pool == null) return;

        timer += Time.deltaTime;
        if (timer >= table.spawnInterval && alive < table.maxAlive)
        {
            timer = 0f;
            Spawn(Pick());
        }
    }

    SpawnTableSO.Entry Pick()
    {
        int total = 0;
        foreach (var e in table.entries) total += e.weight;
        if (total <= 0) return null;
        int r = Random.Range(0, total);
        foreach (var e in table.entries)
            if ((r -= e.weight) < 0) return e;
        return null;
    }

    void Spawn(SpawnTableSO.Entry e)
    {
        if (e == null || e.enemy == null || e.enemy.prefab == null) return;

        var point = (spawnPoints != null && spawnPoints.Length > 0)
            ? spawnPoints[Random.Range(0, spawnPoints.Length)]
            : transform;

        // ✅ spawn from pool (no Instantiate)
        var go = pool.Spawn(e.enemy.prefab, point.position, point.rotation);

        // init enemy stats
        var enemy = go.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Init(e.enemy); // reapply HP/speed per spawn
            alive++;
            // decrement alive when this enemy dies (despawns)
            enemy.OnDeath += () => { alive--; };
        }
        else
        {
            Debug.LogWarning("[EnemySpawner] Spawned object has no Enemy component.");
        }
    }
}
