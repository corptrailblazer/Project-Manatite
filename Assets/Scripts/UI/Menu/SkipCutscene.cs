using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Listens for any-key during the opening cutscene and skips ahead to the
/// menu state. Plays the "SkipCutscene" animator state on each skippable object,
/// hard-positions the title ship, and triggers menu reveal animations.
///
/// Refactor notes: animator references resolved once in <see cref="Awake"/>
/// instead of per-keypress; <c>skipable</c> initialised in the field declaration
/// rather than in an empty <c>Start</c>.
/// </summary>
public class SkipCutscene : MonoBehaviour
{
    [SerializeField] private List<GameObject> objects = new();
    [SerializeField] private GameObject ship;
    [SerializeField] private TriggerMovement menuHandler;
    [SerializeField] private StartMenuAnimation background;
    [SerializeField] private AudioSource audioObj;
    [SerializeField] private string skipAnimationStateName = "SkipCutscene";
    [SerializeField] private Vector3 shipFinalPosition = new(0f, 0.314f, 0f);
    [SerializeField] private Vector3 shipFinalScale = new(0.17f, 0.17f, 0f);

    private Animator[] cachedAnimators;
    private PseudoAnimation shipScript;
    private bool skipable = true;

    private void Awake()
    {
        cachedAnimators = new Animator[objects.Count];
        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[i] != null)
                cachedAnimators[i] = objects[i].GetComponent<Animator>();
        }
        if (ship != null) shipScript = ship.GetComponent<PseudoAnimation>();
    }

    private void Update()
    {
        if (!skipable) return;

        bool keyPressed = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
        bool mousePressed = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

        if (!keyPressed && !mousePressed) return;

        skipable = false;

        for (int i = 0; i < cachedAnimators.Length; i++)
        {
            var a = cachedAnimators[i];
            if (a != null) a.Play(skipAnimationStateName, 0, 1f);
        }

        if (ship != null)
        {
            ship.transform.position = shipFinalPosition;
            ship.transform.localScale = shipFinalScale;
        }

        if (shipScript != null)
        {
            shipScript.OnAnimationEnd();
            shipScript.RemoveParticles();
        }

        if (menuHandler != null) menuHandler.StartMenu(true);
        if (background != null) background.TriggerStartMoving();
    }
}
