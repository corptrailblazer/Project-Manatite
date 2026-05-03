using UnityEngine;
using System.Collections;

public class AnimationShopKeeper: MonoBehaviour
{

    Animator animator;
    string curAnim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>(); 
    }

    // Update is called once per frame
    void Update()
    {
    }

}
