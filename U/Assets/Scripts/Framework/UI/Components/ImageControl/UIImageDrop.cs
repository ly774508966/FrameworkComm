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
    public class UIImageDrop : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Action<GameObject> onDrop;

        private Image _droppableImage;

        private void OnEnable()
        {
            _droppableImage = GetComponent<Image>();
        }

        public void OnDrop(PointerEventData eventData)
        {
            GameObject dropGameObject = eventData.pointerDrag;
            if (dropGameObject != null)
            {
                Image dropImage = dropGameObject.GetComponent<Image>();
                if (dropImage != null && dropImage.sprite != null)
                {
                    _droppableImage.overrideSprite = dropImage.sprite;
                    _droppableImage.enabled = true;
                    _droppableImage.color = Color.white;
                    onDrop.Call(dropGameObject);
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // to do
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // to do
        }
    }
}