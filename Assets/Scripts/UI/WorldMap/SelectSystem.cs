using System.Collections;
using ProjectManatite.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// World-map level / system selector. Detects a click on this object's collider
/// and loads <see cref="sceneName"/> after a short button-twinkle animation.
///
/// Refactor notes: <c>Camera.main</c> per-frame replaced with
/// <see cref="CameraCache.Main"/>; debug log removed; null-safe against
/// missing input device.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class SelectSystem : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private LayerMask clickableMask = ~0;
    [SerializeField] private ButtonTwinkler twinkler;
    [SerializeField] private float loadDelaySeconds = 2f;

    private Collider2D _col;
    private Camera _cam;

    private void Start()
    {
        _col = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame) return;
        if (_cam == null) _cam = CameraCache.Main;
        if (_cam == null) return;

        Vector2 worldPos = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        var hit = Physics2D.OverlapPoint(worldPos, clickableMask);
        if (hit == _col) StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        if (twinkler != null) StartCoroutine(twinkler.Twinkle());
        yield return new WaitForSeconds(loadDelaySeconds);
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}
