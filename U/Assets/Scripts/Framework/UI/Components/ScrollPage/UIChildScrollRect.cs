using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 继承自 UGUI ScrollRect Component
/// 适用场景: 组合ScrollRect页面，Child-ScrollRect(this) 垂直/水平滚动，而 Parent-ScrollRect 水平/垂直滚动
/// 主要功能: DragEvents 透传
/// by zhenhaiwang
/// </summary>
namespace Framework.UI
{
    public class UIChildScrollRect : ScrollRect
    {
        ScrollRect _parentScrollRect;
        bool _isFireToParent = false;

        protected override void Start()
        {
            base.Start();

            // can't do this in Awake function when working with UISimpleObjectPool component
            if (!(_parentScrollRect = transform.parent.GetComponentInParent<ScrollRect>()))
            {
                Log.Error("UIChildScrollRect.Start error: return null when getting parent ScrollRect");
            }
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);

            _isFireToParent = IsFireToParentScrollRect(eventData);
            if (_isFireToParent)
            {
                _parentScrollRect.OnBeginDrag(eventData);
                _parentScrollRect.SendMessage("OnBeginDrag", eventData, SendMessageOptions.DontRequireReceiver);
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (_isFireToParent)
            {
                _parentScrollRect.OnDrag(eventData);
                _parentScrollRect.SendMessage("OnDrag", eventData, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                base.OnDrag(eventData);
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);

            if (_isFireToParent)
            {
                _parentScrollRect.OnEndDrag(eventData);
                _parentScrollRect.SendMessage("OnEndDrag", eventData, SendMessageOptions.DontRequireReceiver);
            }
        }

        private bool IsFireToParentScrollRect(PointerEventData eventData)
        {
            if (_parentScrollRect == null)
                return false;

            if (!_parentScrollRect.vertical && !_parentScrollRect.horizontal)
                return false;

            if (!vertical && !horizontal)
            {
                return true;
            }
            else
            {
                if (horizontal)
                {
                    return Mathf.Abs(eventData.delta.y) > Mathf.Abs(eventData.delta.x);
                }
                else // if (vertical)
                {
                    return Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y);
                }
            }
        }
    }
}
