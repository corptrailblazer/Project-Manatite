using System.Collections;
using ProjectManatite.Core;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tracks kills and triggers the win sequence (progress bar, fade, freeze, disable
/// active spawners).
///
/// <para>Refactor notes vs. the original WinLevel.cs:</para>
/// <list type="bullet">
///   <item>Subscribes to <see cref="GameEvents.EnemyDied"/> so kill counting is event-driven instead of relying on Enemy holding a direct ref to this class.</item>
///   <item><c>GameObject.Find("Spawner")</c> / <c>GameObject.Find("PoolManager")</c> magic-string lookups replaced with serialized references. Both can still be left null and the <see cref="Win"/> sequence still completes.</item>
///   <item>Update polling removed: <see cref="Win"/> is invoked exactly once when the kill threshold is crossed, not every frame after.</item>
///   <item>Magic numbers (4.5f, 0.95f, 1.4f, 1.5f) lifted to named serialized fields.</item>
///   <item>Public <see cref="IncreaseKillCount"/> retained so legacy callers / unit tests can drive kill counts directly.</item>
/// </list>
/// </summary>
public class WinLevel : MonoBehaviour
{
    [Header("Win condition")]
    [SerializeField] private int killCountToWin = 10;

    [Header("UI / FX")]
    [SerializeField] private GameObject win;
    [SerializeField] private Image backgroundFade;
    [SerializeField] private Transform progressBarMask;

    [Header("Optional scene refs (avoid magic-string Finds)")]
    [SerializeField] private GameObject spawnerObject;
    [SerializeField] private GameObject poolManagerObject;

    [Header("Tuning")]
    [SerializeField] private float progressBarLength = 4.5f;
    [SerializeField] private float fadeDurationSeconds = 1.4f;
    [SerializeField] private float fadeTargetAlpha = 0.95f;
    [SerializeField] private float pauseDelaySeconds = 1.5f;

    private int currentKill;
    private Vector3 incrementPos;
    private bool wonAlready;

    private void OnEnable()
    {
        GameEvents.EnemyDied += OnEnemyDiedEvent;
    }

    private void OnDisable()
    {
        GameEvents.EnemyDied -= OnEnemyDiedEvent;
    }

    private void Start()
    {
        if (killCountToWin <= 0) killCountToWin = 1;
        incrementPos = new Vector3(progressBarLength / killCountToWin, 0f, 0f);

        // Best-effort fallback for scenes that haven't been re-wired to the
        // serialized references yet.
        if (spawnerObject == null)     spawnerObject     = GameObject.Find("Spawner");
        if (poolManagerObject == null) poolManagerObject = GameObject.Find("PoolManager");
    }

    private void OnEnemyDiedEvent(GameObject _) => IncreaseKillCount();

    public void IncreaseKillCount()
    {
        if (wonAlready) return;

        if (progressBarMask != null) progressBarMask.localPosition += incrementPos;
        currentKill++;

        if (currentKill >= killCountToWin) Win();
    }

    private void Win()
    {
        if (wonAlready) return;
        wonAlready = true;

        StartCoroutine(FadeRoutine());
        StartCoroutine(PauseGameRoutine());

        if (win != null) win.transform.localPosition = Vector3.zero;
        if (spawnerObject != null)     spawnerObject.SetActive(false);
        if (poolManagerObject != null) poolManagerObject.SetActive(false);

        GameEvents.RaiseBattleWon();
    }

    private IEnumerator PauseGameRoutine()
    {
        yield return new WaitForSeconds(pauseDelaySeconds);
        Time.timeScale = 0f;
    }

    private IEnumerator FadeRoutine()
    {
        if (backgroundFade == null) yield break;

        float t = 0f;
        Color c = backgroundFade.color;
        float startA = c.a;

        while (t < fadeDurationSeconds)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(startA, fadeTargetAlpha, t / fadeDurationSeconds);
            backgroundFade.color = c;
            yield return null;
        }

        c.a = fadeTargetAlpha;
        backgroundFade.color = c;
    }
}
