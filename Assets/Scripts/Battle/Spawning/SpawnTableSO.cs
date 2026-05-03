using UnityEngine;

/// <summary>
/// Designer-authored weighted spawn table. <see cref="EnemySpawner"/> picks an
/// entry per <see cref="spawnInterval"/> and respects <see cref="maxAlive"/>.
/// </summary>
[CreateAssetMenu(menuName = "Enemies/Spawn Table", fileName = "SpawnTableSO")]
public class SpawnTableSO : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public EnemySO enemy;

        [Tooltip("Relative weight against the sum of all entries.")]
        [Range(0, 100)] public int weight = 10;
    }

    public Entry[] entries;

    [Tooltip("Seconds between spawn attempts.")]
    [Min(0f)] public float spawnInterval = 1.5f;

    [Tooltip("Hard cap on living spawns from this table at any time.")]
    [Min(0)] public int maxAlive = 20;
}
