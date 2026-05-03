using System;
using UnityEngine;

namespace ProjectManatite.Core
{
    /// <summary>Designer-friendly int event channel (coin deltas, kill counts, etc.).</summary>
    [CreateAssetMenu(menuName = "Manatite/Events/Int", fileName = "IntEventChannel")]
    public class IntEventChannelSO : ScriptableObject
    {
        public event Action<int> OnRaised;

        public void Raise(int value) => OnRaised?.Invoke(value);
    }
}
