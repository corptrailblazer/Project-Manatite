using UnityEngine;

public class SpritePulse : MonoBehaviour
{
    [SerializeField] float pulseSpeed = 2f;   // how fast it pulses
    [SerializeField] float scaleAmount = 0.25f; // how much it grows/shrinks
    Vector3 baseScale;

    void Awake()
    {
        baseScale = transform.localScale; // store the starting size
    }

    void Update()
    {
        float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * scaleAmount;
        transform.localScale = baseScale * scale;
    }
}
