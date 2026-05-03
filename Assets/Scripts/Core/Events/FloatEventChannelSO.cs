using System;
using UnityEngine;

namespace ProjectManatite.Core
{
    /// <summary>Designer-friendly float event channel (health %, fire rate, etc.).</summary>
    [CreateAssetMenu(menuName = "Manatite/Events/Float", fileName = "FloatEventChannel")]
    public class FloatEventChannelSO : ScriptableObject
    {
        public event Action<float> OnRaised;

        public void Raise(float value) => OnRaised?.Invoke(value);
    }
}
