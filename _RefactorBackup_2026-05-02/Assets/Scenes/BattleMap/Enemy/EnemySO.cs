using UnityEngine;

[CreateAssetMenu(menuName="Enemies/Enemy")]
public class EnemySO : ScriptableObject {
    public GameObject prefab;
    public float hp = 10;
    public float speed = 2;
    public float damage = 1;
    public int qntyMantatite = 0;
}
