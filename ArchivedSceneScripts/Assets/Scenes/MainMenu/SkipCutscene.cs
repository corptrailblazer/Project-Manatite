using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class SkipCutscene : MonoBehaviour
{

    [SerializeField] private List<GameObject> objects = new List<GameObject>();
    [SerializeField] private GameObject ship;
    [SerializeField] private TriggerMovement menuHandler;
    [SerializeField] private StartMenuAnimation background;
    [SerializeField] private AudioSource audioObj;
    private Animator animator;
    private bool skipable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        skipable = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.anyKey.wasPressedThisFrame && skipable)
        {
            foreach (var obj in objects)
            {
                animator = obj.GetComponent<Animator>();
                animator.Play("SkipCutscene", 0, 1f);
            }
            var shipScript = ship.GetComponent<PseudoAnimation>();
            ship.transform.position = new Vector3(0f, 0.314f, 0f);
            ship.transform.localScale = new Vector3(0.17f, 0.17f, 0f);
            shipScript.OnAnimationEnd();
            shipScript.RemoveParticles();

            menuHandler.StartMenu();
            background.TriggerStartMoving();


        }
    }
}
