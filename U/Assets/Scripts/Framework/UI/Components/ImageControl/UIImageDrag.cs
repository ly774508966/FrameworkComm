using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework.UI
{
    [RequireComponent(typeof(Image))]
    public class UIImageDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public bool dragOnSurfaces = false;

        public Action<GameObject> onBeginDrag;
        public Action onEndDrag;

        private Image _draggableImage;
        private RectTransform _draggingRectTransform;
        private RectTransform _draggingPlane;

        private void OnEnable()
        {
            _draggableImage = GetComponent<Image>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Canvas canvas = UIUtils.FindInParent<Canvas>(gameObject);
            if (canvas != null)
            {
                GameObject draggingGameObject = UIUtils.AddChild(canvas.gameObject);
                draggingGameObject.name = gameObject.name;
                draggingGameObject.transform.SetAsLastSibling();

                Image image = draggingGameObject.AddComponent<Image>();
                image.raycastTarget = false;
                image.sprite = _draggableImage.sprite;
                image.SetNativeSize();

                _draggingRectTransform = draggingGameObject.GetComponent<RectTransform>();

                if (dragOnSurfaces)
                {
                    _draggingPlane = transform as RectTransform;
                }
                else
                {
                    _draggingPlane = canvas.transform as RectTransform;
                }

                OnDrag(eventData);

                onBeginDrag.Call(draggingGameObject);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_draggingRectTransform != null)
            {
                if (dragOnSurfaces && eventData.pointerEnter != null)
                {
                    RectTransform pointerEnterRectTransform = eventData.pointerEnter.transform as RectTransform;
                    if (pointerEnterRectTransform != null)
                    {
                        _draggingPlane = pointerEnterRectTransform;
                    }
                }

                Vector3 globalMousePosition;
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_draggingPlane, eventData.position, eventData.pressEventCamera, out globalMousePosition))
                {
                    _draggingRectTransform.position = globalMousePosition;
                    _draggingRectTransform.rotation = _draggingPlane.rotation;
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_draggingRectTransform != null)
            {
                Destroy(_draggingRectTransform.gameObject);
                _draggingRectTransform = null;
                onEndDrag.Call();
            }
        }
    }
}