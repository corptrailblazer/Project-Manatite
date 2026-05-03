using UnityEngine;
using System.Collections;


public class CanvasHandler : MonoBehaviour
{
    CanvasGroup group;
    SpriteRenderer[] sprites;

    void Awake()
    {
        group = GetComponent<CanvasGroup>();
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false; 

        sprites = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);

        foreach (var sr in sprites)
        {
            Color c = sr.color;
            c.a = 0f;
            sr.color = c;
        }
    }

    void Update()
    {
    }

    public IEnumerator FadeIn(float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / duration);
            group.alpha = alpha;

            foreach (var sr in sprites)
            {
                Color c = sr.color;
                c.a = alpha;
                sr.color = c;
            }

            yield return null;
        }
        group.alpha = 1f;
        group.interactable = true;
        group.blocksRaycasts = true; 
    }
}
