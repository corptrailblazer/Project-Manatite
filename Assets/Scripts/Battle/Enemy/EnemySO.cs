using UnityEngine;

/// <summary>
/// Designer-authored stat block for an enemy archetype.
/// Drop into <see cref="SpawnTableSO"/> entries; the spawner instantiates
/// <see cref="prefab"/> and copies stats via <see cref="Enemy.Init"/>.
/// </summary>
[CreateAssetMenu(menuName = "Enemies/Enemy", fileName = "EnemySO")]
public class EnemySO : ScriptableObject
{
    [Tooltip("Visual / collider prefab. Must have an Enemy component.")]
    public GameObject prefab;

    [Min(1)] public float hp = 10;
    [Min(0)] public float speed = 2;
    [Min(0)] public float damage = 1;

    [Tooltip("How many Manatite (coins) the player earns when this enemy dies.")]
    [Min(0)] public int qntyMantatite = 0;
}
