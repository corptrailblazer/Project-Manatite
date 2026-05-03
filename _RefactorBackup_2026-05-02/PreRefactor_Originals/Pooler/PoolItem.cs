// PoolItem.cs (stores original prefab key)
using UnityEngine;

public class PoolItem : MonoBehaviour
{
    [HideInInspector] public GameObject prefabKey;
    [HideInInspector] public PoolManager manager;

    void OnDisable() { /* safety if needed */ }
}
