using UnityEngine;
using System.Collections.Generic;

public class StartMenuAnimation : MonoBehaviour
{

    [SerializeField] List<MenuScrolling> targets = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerStartMoving()
    {
        foreach (var t in targets)
            if (t != null)
                t.startMoving();
    }
}
