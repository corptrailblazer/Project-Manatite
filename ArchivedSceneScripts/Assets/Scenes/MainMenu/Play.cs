using UnityEngine;
using UnityEngine.SceneManagement;

public class Play : MonoBehaviour
{
    // Chame essa função no OnClick do botão
    public void LoadScene()
    {
        SceneManager.LoadScene("EntreMapas");
    }
}
