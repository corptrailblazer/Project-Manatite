using UnityEngine;
using System.Collections.Generic;

public class TriggerMovement : MonoBehaviour
{

    [SerializeField] List<PseudoMovimentoFogo> targets = new();
    [SerializeField] PseudoAnimation psudeoAnimation;
    [SerializeField] CanvasHandler ch;
    [SerializeField] List<MenuScrolling> scrollings = new();

    float x = 0.4f;
    float y = 0.5f;
    float scaleX = 1.2f;
    float scaleY = 1.1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TriggerMovementIncrease()
    {
        foreach (var t in targets)
            if (t != null)
                t.changeIntensity(x, y, scaleX, scaleY, new Vector3(-0.5f, 0, 0));

        foreach (var s in scrollings)
            if (s != null)
                s.increaseSpeed(2f);
        //psudeoAnimation.increaseSpeed(1f);
       
    }

    void TriggerMovementDecrease()
    {
        foreach (var t in targets)
            if (t != null)
                t.changeIntensity(-x, -y, -scaleX, -scaleY, new Vector3(0.5f, 0, 0));
        
        foreach (var s in scrollings)
            if (s != null)
                s.decreaseSpeed(2f);
        //psudeoAnimation.decreaseSpeed(-1f);
       
    }

    public void StartMenu()
    {
        StartCoroutine(ch.FadeIn(3f));
    }
}
