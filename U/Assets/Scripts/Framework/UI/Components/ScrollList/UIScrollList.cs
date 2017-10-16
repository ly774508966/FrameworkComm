using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Scroll list, support jump-to or scroll-to function
/// @zhenhaiwang
/// </summary>
namespace Framework.UI
{
    public class UIScrollList : UIBaseList
    {
        public ScrollRect scroll;
        public float scrollSmoothing = 8f;

        bool _isHorizantal = false;
        bool _isVertical = false;
        bool _isAutoScrolling = false;

        float _horizantalTargetPos = 1f;
        float _verticalTargetPos = 1f;

        float _len = 1f;
        float _stepLength = 0f;

        protected override void InitView()
        {
            base.InitView();

            _isHorizantal = scroll.horizontal;
            _isVertical = scroll.vertical;
        }

        protected override void OnDataChanged()
        {
            if (datas != null)
            {
                _len = datas.Length;
                if (_len > 1)
                {
                    _stepLength = 1.0f / (_len - 1);
                }
                else
                {
                    _stepLength = 0f;
                }
            }
            else
            {
                _len = 0;
                _stepLength = 0f;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (_isAutoScrolling)
            {
                if (_isVertical)
                {
                    if (Mathf.Abs(scroll.verticalNormalizedPosition - _verticalTargetPos) <= 0.001f)
                    {
                        scroll.verticalNormalizedPosition = _verticalTargetPos;
                        _isAutoScrolling = false;
                    }
                    else
                    {
                        scroll.verticalNormalizedPosition = Mathf.Lerp(scroll.verticalNormalizedPosition, _verticalTargetPos, Time.deltaTime * scrollSmoothing);
                    }
                }
                if (_isHorizantal)
                {
                    if (Mathf.Abs(scroll.horizontalNormalizedPosition - _horizantalTargetPos) <= 0.001f)
                    {
                        scroll.horizontalNormalizedPosition = _horizantalTargetPos;
                        _isAutoScrolling = false;
                    }
                    else
                    {
                        scroll.horizontalNormalizedPosition = Mathf.Lerp(scroll.horizontalNormalizedPosition, _horizantalTargetPos, Time.deltaTime * scrollSmoothing);
                    }
                }
            }
        }

        public void ScrollToTopSmooth()
        {
            _isAutoScrolling = true;
            _verticalTargetPos = 1f;
            _horizantalTargetPos = 0f;
        }

        public void ScrollToTopImmediate()
        {
            if (_isVertical)
                scroll.verticalNormalizedPosition = 1f;
            if (_isHorizantal)
                scroll.horizontalNormalizedPosition = 0f;
        }

        public void ScrollToIndex(int index, bool immediate = false)
        {
            if (index < 0)
            {
                if (immediate)
                {
                    if (_isVertical)
                        scroll.verticalNormalizedPosition = 1f;
                    if (_isHorizantal)
                        scroll.horizontalNormalizedPosition = 0f;
                }
                else
                {
                    _isAutoScrolling = true;
                    _verticalTargetPos = 1f;
                    _horizantalTargetPos = 0f;
                }
            }
            else if (index > _len)
            {
                if (immediate)
                {
                    if (_isVertical)
                        scroll.verticalNormalizedPosition = 0f;
                    if (_isHorizantal)
                        scroll.horizontalNormalizedPosition = 1f;
                }
                else
                {
                    _isAutoScrolling = true;
                    _verticalTargetPos = 0f;
                    _horizantalTargetPos = 1f;
                }
            }
            else if (_len > 0)
            {
                if (immediate)
                {
                    if (_isVertical)
                        scroll.verticalNormalizedPosition = 1f - index * _stepLength;
                    if (_isHorizantal)
                        scroll.horizontalNormalizedPosition = index * _stepLength;
                }
                else
                {
                    _isAutoScrolling = true;
                    _verticalTargetPos = 1.0f - index * _stepLength;
                    _horizantalTargetPos = index * _stepLength;
                }
            }
        }
    }
}
