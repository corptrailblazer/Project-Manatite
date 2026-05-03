using UnityEngine;
using UnityEngine.SceneManagement;


public class StateManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    public void Abort()
    {
        SceneManager.LoadScene("EntreMapas");
        Time.timeScale = 1f;
    }

    public void Continue()
    {
        SceneManager.LoadScene("Shop");
        Time.timeScale = 1f;
    }
}
