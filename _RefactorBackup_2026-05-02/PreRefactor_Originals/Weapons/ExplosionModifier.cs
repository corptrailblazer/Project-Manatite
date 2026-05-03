using UnityEngine;

public class ExplosionModifier : MonoBehaviour
{
    [SerializeField] ExplosionModifierSO SO;
    void OnTriggerEnter2D(Collider2D other)
    {
       if (!other.CompareTag("Enemy")) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(SO.damage);
        }

    }
}
