using UnityEngine;

[CreateAssetMenu(menuName="Enemies/Spawn Table")]
public class SpawnTableSO : ScriptableObject {
    [System.Serializable]
    public class Entry {
        public EnemySO enemy;
        [Range(0,100)] public int weight = 10; // chance relativa
    }
    public Entry[] entries;
    public float spawnInterval = 1.5f; // s
    public int maxAlive = 20;          // limite em cena
}
