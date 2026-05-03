using UnityEngine;
using System.Collections;

public class MenuScrolling : MonoBehaviour
{
    public float speed = 2f;         // velocidade do movimento
    public float width = 10f;        // largura total do sprite/objeto em unidades
    public float offset = 0f;  
    public Vector3 direction = Vector3.left; // direção do movimento

    public Vector3 startPos;
    bool move;


    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (move == false) return;
        transform.position += direction.normalized * speed * Time.deltaTime;

        // se passou da largura definida, volta pro começo
        if (Vector3.Distance(startPos, transform.position) >= width)
        {
            startPos.x += offset;
            width += offset;
            offset = 0;
            transform.position = startPos;

        }

    }

    public void startMoving()
    {
        if (animator != null) animator.enabled = false;
        move = true;
    }

    public void increaseSpeed(float speedo)
    {
        StartCoroutine(changeSpeed(speedo, 2f, true));
    }

    public void decreaseSpeed(float speedo)
    {
        StartCoroutine(changeSpeed(-speedo, 1f, false));
    }


    IEnumerator changeSpeed(float value, float duration, bool isIncrease)
    {
        var target = (speed + value);
        if (isIncrease)
        {
            while (speed < target)
            {
                speed += (value / (duration / Time.deltaTime));

                yield return null;
            }

        }

        if (!isIncrease)
        {
            while (speed > target)
            {
                speed += (value / (duration / Time.deltaTime));

                yield return null;
            }

        }
            
    }

}
