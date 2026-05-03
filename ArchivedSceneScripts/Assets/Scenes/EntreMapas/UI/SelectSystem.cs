using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class SelectSystem : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] LayerMask clickableMask = ~0;
    [SerializeField] ButtonTwinkler twinkler;

    private Collider2D col;

    void Start()
    {
        col = GetComponent<Collider2D>();
    }

     void Update()
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        Collider2D hit = Physics2D.OverlapPoint(worldPos, clickableMask);

        if (Mouse.current.leftButton.wasPressedThisFrame && hit != null && hit == col)
        {
            Debug.Log("FAAAAAAA");
            StartCoroutine(StartGame());
        }
            


    }

    IEnumerator StartGame()
    {
        StartCoroutine(twinkler.Twinkle());
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(sceneName);
    }

}
