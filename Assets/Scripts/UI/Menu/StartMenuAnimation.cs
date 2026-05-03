using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Convenience entry point — fans out a "start moving" trigger to a list of
/// <see cref="MenuScrolling"/> background layers.
/// Empty Start / Update from the original removed.
/// </summary>
public class StartMenuAnimation : MonoBehaviour
{
    [SerializeField] private List<MenuScrolling> targets = new();

    public void TriggerStartMoving()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            var t = targets[i];
            if (t != null) t.startMoving();
        }
    }
}
