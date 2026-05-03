using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonTwinkler : MonoBehaviour
{
    [SerializeField] float scaleUpFactor = 1.2f; // how big it gets
    [SerializeField] float animationDuration = 0.2f; // total animation time

    Button button;
    Vector3 originalScale;

    void Awake()
    {
        button = GetComponent<Button>();
        originalScale = transform.localScale;

        button.onClick.AddListener(() => StartCoroutine(Twinkle()));
    }

    public IEnumerator Twinkle()
    {
        float t = 0f;
        Vector3 targetScale = originalScale * scaleUpFactor;

        // Scale up
        while (t < animationDuration / 2f)
        {
            t += Time.deltaTime;
            float lerp = t / (animationDuration / 2f);
            transform.localScale = Vector3.Lerp(originalScale, targetScale, lerp);
            yield return null;
        }

        t = 0f;
        // Scale down
        while (t < animationDuration / 2f)
        {
            t += Time.deltaTime;
            float lerp = t / (animationDuration / 2f);
            transform.localScale = Vector3.Lerp(targetScale, originalScale, lerp);
            yield return null;
        }

        transform.localScale = originalScale;
    }
}
