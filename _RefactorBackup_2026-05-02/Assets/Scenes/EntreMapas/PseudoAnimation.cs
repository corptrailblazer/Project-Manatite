using UnityEngine;
using System.Collections;

public class PseudoAnimation : MonoBehaviour
{
    [Header("Configurações")]
    public float amplitude = 4f;
    public float speed = 2f;
    public int pixelsPerUnit = 32;

    Vector3 startPos;

    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        startPos = transform.localPosition;
    }
    
    void Update()
    {

        float offset = (amplitude / pixelsPerUnit) * Mathf.Sin((Time.time) * speed);

        transform.localPosition = startPos + new Vector3(0, offset, 0);

    }

    public void OnAnimationEnd()
    {
        animator.enabled = false;
        startPos = new Vector3(0f, 0.314f, 0f);
        
    }

    public void RemoveParticles()
    {
        var ps = GetComponent<ParticleSystem>();
        ps.Stop();
    }

    public void increaseSpeed(float speedo)
    {
        speed += speedo;
    }

    public void decreaseSpeed(float speedo)
    {
        speed += speedo;
    }


}
