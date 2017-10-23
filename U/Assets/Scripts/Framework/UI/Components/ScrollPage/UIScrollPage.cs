using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework.UI
{
    public class UIScrollPage : BaseView, IBeginDragHandler, IEndDragHandler
    {
        public delegate void PageChangeDelegate(int currentPageIndex);

        public enum Direction { Vertical, Horizontal }

        // page direction, from left to right, or from top to bottom
        public Direction direction = Direction.Horizontal;

        // page ScrollRect component
        public ScrollRect pageScrollRect;

        // content transform (item container)
        public Transform pageContent;

        // the smoothness when auto scrolling to the next page
        public float scrollSmoothing = 8.0f;

        // the minimum sliding speed determining whether auto scroll to the next page or not
        public float nextPageThreshold = 0.3f;

        // enable drag scrolling to next page
        public bool dragScrollEnabled = true;

        // page OnValueChange delegate
        public PageChangeDelegate OnPageChange = null;

        // page current index
        protected int currentPageIndex
        {
            get { return _currentPageIndex; }
            set
            {
                _currentPageIndex = value;

                if (OnPageChange != null)
                {
                    OnPageChange(value);
                }
            }
        }

        // page total count
        protected int currentPageCount
        {
            get { return pageContent.childCount; }
        }

        private int _currentPageIndex = 0;

        private bool _isAutoScrolling = false;

        private float _targetPosition = 0.0f;

        private float _beginDragTime = 0.0f;

        protected override void Start()
        {
            EnableDragScroll(dragScrollEnabled);
        }

        protected override void Update()
        {
            base.Update();

            if (!_isAutoScrolling)
                return;

            switch (direction)
            {
                case Direction.Vertical:
                    if (Mathf.Abs(pageScrollRect.verticalNormalizedPosition - _targetPosition) <= 0.001f) // Magic number based on what "feels right"
                    {
                        pageScrollRect.verticalNormalizedPosition = _targetPosition;
                        _isAutoScrolling = false;
                    }
                    else
                    {
                        pageScrollRect.verticalNormalizedPosition = Mathf.Lerp(pageScrollRect.verticalNormalizedPosition, _targetPosition, Time.deltaTime * scrollSmoothing);
                    }
                    break;
                case Direction.Horizontal:
                    if (Mathf.Abs(pageScrollRect.horizontalNormalizedPosition - _targetPosition) <= 0.001f) // Magic number based on what "feels right"
                    {
                        pageScrollRect.horizontalNormalizedPosition = _targetPosition;
                        _isAutoScrolling = false;
                    }
                    else
                    {
                        pageScrollRect.horizontalNormalizedPosition = Mathf.Lerp(pageScrollRect.horizontalNormalizedPosition, _targetPosition, Time.deltaTime * scrollSmoothing);
                    }
                    break;
            }
        }

        /// <summary>
        /// enable or disable drag scrolling to next page
        /// </summary>
        public void EnableDragScroll(bool enabled)
        {
            dragScrollEnabled = enabled;
            pageScrollRect.vertical = direction == Direction.Vertical && dragScrollEnabled;
            pageScrollRect.horizontal = direction == Direction.Horizontal && dragScrollEnabled;
        }

        /// <summary>
        /// jump to target page immediately
        /// </summary>
        public virtual void SetPage(int index)
        {
            if (index >= 0 && index < currentPageCount)
            {
                currentPageIndex = index;

                switch (direction)
                {
                    case Direction.Vertical:
                        pageScrollRect.verticalNormalizedPosition = Mathf.Clamp01(index / (float)(currentPageCount - 1));
                        break;
                    case Direction.Horizontal:
                        pageScrollRect.horizontalNormalizedPosition = Mathf.Clamp01(index / (float)(currentPageCount - 1));
                        break;
                }
            }
        }

        /// <summary>
        /// scroll to target page
        /// </summary>
        public virtual void ScrollToPage(int index)
        {
            if (index >= 0 && index < currentPageCount)
            {
                currentPageIndex = index;

                _isAutoScrolling = true;
                _targetPosition = Mathf.Clamp01(index / (float)(currentPageCount - 1));
            }
        }

        /// <summary>
        /// scroll to target page, after delay time
        /// </summary>
        public virtual void ScrollToPage(int index, float delay, Action callback = null)
        {
            if (index >= 0 && index < currentPageCount)
            {
                if (delay > 0.0f)
                {
                    iTweenUtils.CreateTimeout(gameObject, delegate ()
                    {
                        ScrollToPage(index);
                        callback.Call();
                    }, delay, "UIScrollPage_ScrollToPage");
                }
                else
                {
                    ScrollToPage(index);
                }
            }
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            EnableDragScroll(dragScrollEnabled);

            if (dragScrollEnabled)
            {
                _isAutoScrolling = false;
                _beginDragTime = Time.unscaledTime;
            }
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (currentPageCount <= 1 || _isAutoScrolling)
                return;

            _isAutoScrolling = true;

            float speed = (eventData.position.x - eventData.pressPosition.x) / ((Time.unscaledTime - _beginDragTime) * 1000.0f);

            if (speed >= nextPageThreshold && currentPageIndex > 0)
            {
                _targetPosition = Mathf.Clamp01(--currentPageIndex / (float)(currentPageCount - 1));
            }
            else if (speed <= -nextPageThreshold && currentPageIndex < currentPageCount - 1)
            {
                _targetPosition = Mathf.Clamp01(++currentPageIndex / (float)(currentPageCount - 1));
            }
        }
    }
}
