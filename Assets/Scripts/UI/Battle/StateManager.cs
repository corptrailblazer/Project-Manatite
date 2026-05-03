using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Battle-end menu controller. Wired to UI buttons (Restart / Abort / Continue);
/// each method loads the relevant scene and resets <see cref="Time.timeScale"/>.
///
/// Refactor notes: empty <c>Start</c> removed. Scene names extracted to
/// constants so a typo doesn't fail silently at click time.
/// </summary>
public class StateManager : MonoBehaviour
{
    private const string AbortSceneName = "EntreMapas";
    private const string ContinueSceneName = "Shop";

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Abort()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(AbortSceneName);
    }

    public void Continue()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(ContinueSceneName);
    }
}
