using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Tiny button handler — wired to the main-menu Play button's <c>OnClick</c>.
/// Loads the world-map scene.
/// </summary>
public class Play : MonoBehaviour
{
    [Tooltip("Name of the scene to load when Play is clicked. Must be in Build Settings.")]
    [SerializeField] private string sceneToLoad = "EntreMapas";

    public void LoadScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneToLoad);
    }
}
