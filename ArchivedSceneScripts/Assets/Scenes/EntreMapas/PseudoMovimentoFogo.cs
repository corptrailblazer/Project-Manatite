using UnityEngine;

public class PseudoMovimentoFogo : MonoBehaviour
{
    [Header("Configurações da distorção")]
    public float baseScaleX = 1f;      // largura média
    public float baseScaleY = 1f;      // altura média
    public float amplitudeX = 0.2f;    // intensidade em X
    public float amplitudeY = 0.3f;    // intensidade em Y
    public float speedX = 1.5f;        // velocidade da variação em X
    public float speedY = 2f;          // velocidade da variação em Y
    public bool randomizeOffset = true;

    private float offsetX;
    private float offsetY;

    void Start()
    {
        // offsets para que cada fogo tenha padrão diferente
        offsetX = randomizeOffset ? Random.Range(0f, 100f) : 0f;
        offsetY = randomizeOffset ? Random.Range(0f, 100f) : 10f;
    }

    void Update()
    {
        // PerlinNoise retorna [0..1], então ajustamos para [-1..1]
        float noiseX = (Mathf.PerlinNoise(Time.time * speedX + offsetX, 0f) * 2f - 1f) * amplitudeX;
        float noiseY = (Mathf.PerlinNoise(Time.time * speedY + offsetY, 0f) * 2f - 1f) * amplitudeY;

        Vector3 scale = transform.localScale;
        scale.x = baseScaleX + noiseX;
        scale.y = baseScaleY + noiseY;
        transform.localScale = scale;
    }

    public void changeIntensity(float x, float y, float ScaleX, float ScaleY, Vector3 offset)
    {
        amplitudeX += x;
        amplitudeY += y;
        baseScaleX += ScaleX;
        baseScaleY += ScaleY;
        transform.localPosition = transform.localPosition + offset;
    }
}
