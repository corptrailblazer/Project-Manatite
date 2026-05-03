using UnityEngine;

namespace ProjectManatite.Core
{
    /// <summary>
    /// Shared, pre-allocated buffers for Physics2D queries. Replaces allocating
    /// versions like <c>Physics2D.OverlapCircleAll</c> with the
    /// <c>Physics2D.OverlapCircleNonAlloc</c> family.
    ///
    /// All buffers are intentionally not thread-safe — Physics2D is main-thread-only.
    /// Buffer size grows on demand if a query would exceed it.
    /// </summary>
    public static class Physics2DBuffers
    {
        /// <summary>Default starting size; tuned for a few dozen overlaps.</summary>
        private const int DefaultColliderBufferSize = 32;

        private static Collider2D[] _colliders = new Collider2D[DefaultColliderBufferSize];
        private static RaycastHit2D[] _hits = new RaycastHit2D[DefaultColliderBufferSize];

        /// <summary>
        /// Returns the shared collider buffer, growing it (power-of-two) if the caller
        /// hints they need more space. Hits are written into the returned array; the
        /// caller is responsible for using only the count returned by NonAlloc APIs.
        /// </summary>
        public static Collider2D[] GetColliderBuffer(int minSize = DefaultColliderBufferSize)
        {
            if (_colliders.Length < minSize)
            {
                int newSize = Mathf.NextPowerOfTwo(minSize);
                _colliders = new Collider2D[newSize];
            }
            return _colliders;
        }

        /// <summary>Same as <see cref="GetColliderBuffer"/> but for raycast hits.</summary>
        public static RaycastHit2D[] GetHitBuffer(int minSize = DefaultColliderBufferSize)
        {
            if (_hits.Length < minSize)
            {
                int newSize = Mathf.NextPowerOfTwo(minSize);
                _hits = new RaycastHit2D[newSize];
            }
            return _hits;
        }
    }
}
