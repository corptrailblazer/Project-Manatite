using UnityEngine;

namespace ProjectManatite.Core
{
    /// <summary>
    /// Caches <see cref="Camera.main"/> so callers don't pay the FindGameObjectsWithTag
    /// cost every frame. <c>Camera.main</c> is O(n) in scene-tagged cameras under the
    /// hood; calling it from <c>Update</c> is a known hot-path issue.
    ///
    /// Usage: <c>var cam = CameraCache.Main;</c>
    ///
    /// The cache self-heals: if the cached camera is destroyed (scene reload, camera
    /// swap), the next access transparently re-queries.
    /// </summary>
    public static class CameraCache
    {
        private static Camera _main;

        /// <summary>Cached main camera. Re-queries if the cached reference died.</summary>
        public static Camera Main
        {
            get
            {
                if (_main == null) _main = Camera.main;
                return _main;
            }
        }

        /// <summary>Force-clears the cache. Call after deliberately swapping main cameras.</summary>
        public static void Invalidate() => _main = null;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOnDomainReload() => _main = null;
    }
}
