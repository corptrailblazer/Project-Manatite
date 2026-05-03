using System.Collections.Generic;
using UnityEngine;

namespace ProjectManatite.Core
{
    /// <summary>
    /// Cache of <see cref="WaitForSeconds"/> instances keyed by duration.
    /// Coroutine code like <c>yield return new WaitForSeconds(0.1f);</c> allocates
    /// every iteration — using <c>WaitCache.Seconds(0.1f)</c> reuses one instance.
    ///
    /// Only use for "stable" durations that recur. For unique one-off waits the
    /// allocation cost is negligible.
    /// </summary>
    public static class WaitCache
    {
        private static readonly Dictionary<float, WaitForSeconds> _seconds =
            new Dictionary<float, WaitForSeconds>(16);

        private static WaitForEndOfFrame _endOfFrame;
        private static WaitForFixedUpdate _fixedUpdate;

        public static WaitForSeconds Seconds(float duration)
        {
            if (!_seconds.TryGetValue(duration, out var wait))
            {
                wait = new WaitForSeconds(duration);
                _seconds[duration] = wait;
            }
            return wait;
        }

        public static WaitForEndOfFrame EndOfFrame =>
            _endOfFrame ??= new WaitForEndOfFrame();

        public static WaitForFixedUpdate FixedUpdate =>
            _fixedUpdate ??= new WaitForFixedUpdate();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOnDomainReload()
        {
            _seconds.Clear();
            _endOfFrame = null;
            _fixedUpdate = null;
        }
    }
}
