using UnityEngine;

/// <summary>
/// Shopkeeper animation hook (currently a stub — animator is cached but no
/// per-frame logic). Empty <c>Update</c> from the original removed.
/// </summary>
public class AnimationShopKeeper : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
}
