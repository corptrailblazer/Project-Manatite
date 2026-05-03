using System.Collections;
using UnityEngine;

/// <summary>
/// Fades a CanvasGroup (and any sprite-renderer children) in from invisible.
/// Used by <see cref="TriggerMovement.StartMenu"/> to reveal the main menu UI.
///
/// Refactor notes: empty <c>Update</c> removed; cached components in <see cref="Awake"/>;
/// <c>FadeIn</c> early-outs if the duration is non-positive.
/// </summary>
public class CanvasHandler : MonoBehaviour
{
    private CanvasGroup group;
    private SpriteRenderer[] sprites;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        if (group != null)
        {
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;
        }

        sprites = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
        for (int i = 0; i < sprites.Length; i++)
        {
            var sr = sprites[i];
            Color c = sr.color;
            c.a = 0f;
            sr.color = c;
        }
    }

    public IEnumerator FadeIn(float duration)
    {
        if (group != null)
        {
            group.interactable = true;
            group.blocksRaycasts = true;
        }

        if (duration <= 0f) duration = 0.0001f;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / duration);

            if (group != null) group.alpha = alpha;
            for (int i = 0; i < sprites.Length; i++)
            {
                Color c = sprites[i].color;
                c.a = alpha;
                sprites[i].color = c;
            }

            yield return null;
        }

        if (group != null) group.alpha = 1f;
    }
}
