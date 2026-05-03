using UnityEngine;

/// <summary>
/// Tiny button handler — wired to the main-menu Quit button. Stops play mode in
/// the editor and exits the standalone build.
/// </summary>
public class Quit : MonoBehaviour
{
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
