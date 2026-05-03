using UnityEngine;

public class AnimatorLooper : MonoBehaviour
{
    private Animator anim;

    [Header("Random pause range (seconds)")]
    public float minPause = 0.5f;
    public float maxPause = 2f;

    [Header("Random speed range (seconds)")]
    public float minSpeed = 0.8f;
    public float maxSpeed = 1.5f;

    private bool isWaiting = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isWaiting) return;

        var st = anim.GetCurrentAnimatorStateInfo(0);
        // when the active state finished (not transitioning)
        if (!anim.IsInTransition(0) && st.normalizedTime >= 1f)
        {
            StartCoroutine(PauseAndRestart());
        }
    }

    private System.Collections.IEnumerator PauseAndRestart()
    {
        if (isWaiting) yield break;
        isWaiting = true;

        // capture the current state's full path
        var st = anim.GetCurrentAnimatorStateInfo(0);
        int stateHash = st.fullPathHash;

        // pause
        anim.speed = 0f;

        // random pause
        float pause = Random.Range(minPause, maxPause);
        yield return new WaitForSeconds(pause);

        // rewind to start of the SAME state
        anim.Play(stateHash, 0, 0f);

        // let Animator apply the new state/time
        yield return null;      // allow one frame
        anim.Update(0f);        // (extra safety)

        // resume with random speed
        anim.speed = Random.Range(minSpeed, maxSpeed);

        isWaiting = false;
    }


    void Start()
    {
        float pause = Random.Range(minPause, maxPause);
        StartCoroutine(InitialDelay(pause));
    }

private System.Collections.IEnumerator InitialDelay(float delay)
    {
        anim.speed = 0f;
        yield return new WaitForSeconds(delay);
        anim.speed = 1f;
    }

}


