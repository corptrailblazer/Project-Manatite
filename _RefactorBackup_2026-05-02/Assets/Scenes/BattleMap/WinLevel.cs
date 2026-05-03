using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WinLevel : MonoBehaviour
{
    [SerializeField] int killCountToWin;
    int currentKill;

    [SerializeField] GameObject win;
    [SerializeField] Image backgroundFade;
    [SerializeField] Transform progressBarMask;
    Vector3 incrementPos;
    // 0.05 inicio 4.55 final
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        incrementPos = new Vector3(4.5f/killCountToWin, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentKill >= killCountToWin)
        {
            Win();
        }

    }

    public void IncreaseKillCount()
    {
        progressBarMask.localPosition += incrementPos;
        currentKill += 1;
    }

    void Win()
    {
        StartCoroutine(FadeRoutine());
        StartCoroutine(PauseGame());
        win.transform.localPosition = new Vector3(0f, 0f, 0f);
        GameObject.Find("Spawner").SetActive(false);;
        GameObject.Find("PoolManager").SetActive(false);;
    }

    IEnumerator PauseGame()
    {
        yield return new WaitForSeconds(1.5f);
        Time.timeScale = 0f;
    }

    IEnumerator FadeRoutine()
    {
        float start = 0f;
        float end = 0.95f;
        float t = 0f;
        float duration = 1.4f;

        Color c = backgroundFade.color;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(start, end, t / duration);
            backgroundFade.color = c;
            yield return null;
        }

        c.a = end;
        backgroundFade.color = c;
    }
}
