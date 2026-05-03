using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Christina.UI
{
    /// <summary>
    /// Horizontal carousel with eased lerp between entries and an optional
    /// auto-advance timer. Source preserved at the request of the original
    /// (third-party-style namespace).
    ///
    /// Refactor notes: Layout-rebuild moved out of the per-entry loop so it
    /// runs once after every entry has been instantiated; the original called
    /// <c>LayoutRebuilder.ForceRebuildLayoutImmediate</c> + <c>Canvas.ForceUpdateCanvases</c>
    /// inside the loop, which is O(n²) layout work. For-index iteration replaces
    /// foreach to avoid IEnumerator allocations.
    /// </summary>
    public class Carousel : MonoBehaviour
    {
        [Header("Parts Setup")]
        [SerializeField, Min(1)] private int entryCount = 3;

        [Space]
        [SerializeField] private ScrollRect scrollRect;

        [Space]
        [SerializeField] private RectTransform contentBoxHorizontal;
        [SerializeField] private GameObject carouselEntryPrefab;
        private readonly List<GameObject> _imagesForEntries = new();

        [Space]
        [Header("Animation Setup")]
        [SerializeField, Range(0.25f, 1f)] private float duration = 0.5f;
        [SerializeField] private AnimationCurve easeCurve;

        [Header("Auto Scroll Setup")]
        [SerializeField] private bool autoScroll = false;
        [SerializeField] private float autoScrollInterval = 5f;
        private float _autoScrollTimer;

        private int _currentIndex = 0;
        private Coroutine _scrollCoroutine;

        private void Reset()
        {
            scrollRect = GetComponentInChildren<ScrollRect>();
        }

        private void Start()
        {
            for (int i = 0; i < entryCount; i++)
            {
                var carouselEntry = Instantiate(carouselEntryPrefab, contentBoxHorizontal, false);
                _imagesForEntries.Add(carouselEntry);
            }

            // Single layout pass after all entries are spawned (was per-entry in
            // the original, an O(n²) hit on large carousels).
            if (contentBoxHorizontal != null)
                LayoutRebuilder.ForceRebuildLayoutImmediate(contentBoxHorizontal);
            Canvas.ForceUpdateCanvases();

            _autoScrollTimer = autoScrollInterval;
        }

        public void ScrollToNext()
        {
            if (_imagesForEntries.Count == 0) return;
            _currentIndex = (_currentIndex + 1) % _imagesForEntries.Count;
            ScrollTo(_currentIndex);
        }

        public void ScrollToPrevious()
        {
            if (_imagesForEntries.Count == 0) return;
            _currentIndex = (_currentIndex - 1 + _imagesForEntries.Count) % _imagesForEntries.Count;
            ScrollTo(_currentIndex);
        }

        private void ScrollTo(int index)
        {
            _currentIndex = index;
            _autoScrollTimer = autoScrollInterval;

            int denom = Mathf.Max(1, _imagesForEntries.Count - 1);
            float targetHorizontalPosition = (float)_currentIndex / denom;

            if (_scrollCoroutine != null) StopCoroutine(_scrollCoroutine);
            _scrollCoroutine = StartCoroutine(LerpToPos(targetHorizontalPosition));
        }

        private IEnumerator LerpToPos(float targetHorizontalPosition)
        {
            if (scrollRect == null) yield break;

            float elapsedTime = 0f;
            float initialPos = scrollRect.horizontalNormalizedPosition;

            if (duration > 0)
            {
                while (elapsedTime <= duration)
                {
                    float easeValue = easeCurve.Evaluate(elapsedTime / duration);
                    scrollRect.horizontalNormalizedPosition = Mathf.Lerp(initialPos, targetHorizontalPosition, easeValue);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
            }

            scrollRect.horizontalNormalizedPosition = targetHorizontalPosition;
        }

        private void Update()
        {
            if (!autoScroll) return;

            _autoScrollTimer -= Time.deltaTime;
            if (_autoScrollTimer <= 0f)
            {
                ScrollToNext();
                _autoScrollTimer = autoScrollInterval;
            }
        }
    }
}
