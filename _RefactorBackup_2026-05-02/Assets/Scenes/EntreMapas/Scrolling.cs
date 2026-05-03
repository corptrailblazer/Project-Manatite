using UnityEngine;

public class Scrolling : MonoBehaviour
{
    public float speed = 2f;         // velocidade do movimento
    public float width = 10f;        // largura total do sprite/objeto em unidades
    public float offset = 0f;  
    public Vector3 direction = Vector3.left; // direção do movimento

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
        
    }

    void Update()
    {
        // move o objeto
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
}
