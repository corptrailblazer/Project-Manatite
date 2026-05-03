using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

namespace Christina.UI
{
    public class Carousel : MonoBehaviour
    {
        [Header("Parts Setup")]
        [SerializeField] private List<CarouselSO> entries = new List<CarouselSO>();
        
        [Space]
        [SerializeField] private ScrollRect scrollRect;
     
        [Space]
        [SerializeField] private RectTransform contentBoxHorizontal;
        [SerializeField] private GameObject carouselEntryPrefab;
        private List<GameObject> _imagesForEntries = new List<GameObject>();
        
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
            foreach (var entry in entries)
            {
                GameObject carouselEntry = Instantiate(carouselEntryPrefab, contentBoxHorizontal, false);
                _imagesForEntries.Add(carouselEntry);

                LayoutRebuilder.ForceRebuildLayoutImmediate(contentBoxHorizontal);
                Canvas.ForceUpdateCanvases();
                
            }
            
            _autoScrollTimer = autoScrollInterval;
            
        }
        
        private void ScrollToSpecificIndex(int index)
        {
            
            ScrollTo(index);
        }

        public void ScrollToNext()
        {
            
            _currentIndex = (_currentIndex + 1) % _imagesForEntries.Count;
            ScrollTo(_currentIndex);
        }

        public void ScrollToPrevious()
        {
            
            _currentIndex = (_currentIndex - 1 + _imagesForEntries.Count) % _imagesForEntries.Count;
            ScrollTo(_currentIndex);
        }
        
        private void ScrollTo(int index)
        {
            _currentIndex = index;
            _autoScrollTimer = autoScrollInterval;
            float targetHorizontalPosition = (float)_currentIndex / (_imagesForEntries.Count - 1);
            
            if (_scrollCoroutine != null)
                StopCoroutine(_scrollCoroutine);
            
            _scrollCoroutine = StartCoroutine(LerpToPos(targetHorizontalPosition));
            
        }
        
        private IEnumerator LerpToPos(float targetHorizontalPosition)
        {  
            float elapsedTime = 0f;
            float initialPos = scrollRect.horizontalNormalizedPosition;
            
            if (duration > 0)
            {
                while (elapsedTime <= duration)
                {
                    float easeValue = easeCurve.Evaluate(elapsedTime / duration);

                    float newPosition = Mathf.Lerp(initialPos, targetHorizontalPosition, easeValue);

                    scrollRect.horizontalNormalizedPosition = newPosition;

                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
            }
        
            scrollRect.horizontalNormalizedPosition = targetHorizontalPosition;
        }

        private void Update()
        {
            if (!autoScroll) 
                return;
            
            _autoScrollTimer -= Time.deltaTime;
            if (_autoScrollTimer <= 0)
            {
                ScrollToNext();
                _autoScrollTimer = autoScrollInterval;
            }
        }
        
    }
}