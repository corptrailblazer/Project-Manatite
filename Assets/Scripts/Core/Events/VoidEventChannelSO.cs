using System;
using UnityEngine;

namespace ProjectManatite.Core
{
    /// <summary>
    /// Designer-friendly event channel for parameterless events.
    /// Create assets via <c>Assets &gt; Create &gt; Manatite &gt; Events &gt; Void</c>
    /// and wire them into MonoBehaviours via the inspector.
    /// </summary>
    [CreateAssetMenu(menuName = "Manatite/Events/Void", fileName = "VoidEventChannel")]
    public class VoidEventChannelSO : ScriptableObject
    {
        public event Action OnRaised;

        public void Raise() => OnRaised?.Invoke();
    }
}
