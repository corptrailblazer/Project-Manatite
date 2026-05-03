using System.Collections;
using UnityEngine;

/// <summary>
/// Loops an Animator state with a randomised pause + speed between iterations.
/// On the menu background — used for the twinkling stars / parallax props.
///
/// <para>Refactor notes vs. the original AnimatorPauser.cs:</para>
/// <list type="bullet">
///   <item>Class renamed from <c>AnimatorLooper</c> to match the file name.</item>
///   <item>Cached <see cref="Animator"/> in <see cref="Awake"/> with a null guard.</item>
///   <item>Coroutine waits use plain <see cref="WaitForSeconds"/> (not cached because the duration is randomised).</item>
/// </list>
/// </summary>
public class AnimatorPauser : MonoBehaviour
{
    [Header("Random pause range (seconds)")]
    public float minPause = 0.5f;
    public float maxPause = 2f;

    [Header("Random resume-speed range")]
    public float minSpeed = 0.8f;
    public float maxSpeed = 1.5f;

    private Animator anim;
    private bool isWaiting;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        if (anim == null) return;
        StartCoroutine(InitialDelay(Random.Range(minPause, maxPause)));
    }

    private void Update()
    {
        if (isWaiting || anim == null) return;

        var st = anim.GetCurrentAnimatorStateInfo(0);
        if (!anim.IsInTransition(0) && st.normalizedTime >= 1f)
            StartCoroutine(PauseAndRestart());
    }

    private IEnumerator PauseAndRestart()
    {
        if (isWaiting) yield break;
        isWaiting = true;

        int stateHash = anim.GetCurrentAnimatorStateInfo(0).fullPathHash;

        // Pause the animator.
        anim.speed = 0f;
        yield return new WaitForSeconds(Random.Range(minPause, maxPause));

        // Rewind to the start of the SAME state.
        anim.Play(stateHash, 0, 0f);
        yield return null;     // let Animator apply the new state/time
        anim.Update(0f);

        // Resume with random speed.
        anim.speed = Random.Range(minSpeed, maxSpeed);
        isWaiting = false;
    }

    private IEnumerator InitialDelay(float delay)
    {
        anim.speed = 0f;
        yield return new WaitForSeconds(delay);
        anim.speed = 1f;
    }
}
