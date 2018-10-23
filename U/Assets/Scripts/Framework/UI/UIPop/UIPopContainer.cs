using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework.UI
{
    public sealed class UIPopContainer : MonoBehaviour
    {
        public GameObject child { get; private set; }
        public RawImage mask { get; private set; }

        public string Path { get; set; }
        public bool Modal { get; set; }
        public Action<string> DestroyDelegate { get; set; }

        private void Awake()
        {
            RectTransform containerRect = gameObject.AddComponent<RectTransform>();
            containerRect.pivot = new Vector2(0.5f, 0.5f);
            containerRect.anchorMin = Vector2.zero;
            containerRect.anchorMax = Vector2.one;
            containerRect.offsetMin = Vector2.zero;
            containerRect.offsetMax = Vector2.zero;

            GameObject maskObject = UnityUtils.AddChild(gameObject);
            maskObject.name = "Mask";

            RectTransform maskRect = maskObject.AddComponent<RectTransform>();
            maskRect.pivot = new Vector2(0.5f, 0.5f);
            maskRect.anchorMin = Vector2.zero;
            maskRect.anchorMax = Vector2.one;
            maskRect.offsetMin = Vector2.zero;
            maskRect.offsetMax = Vector2.zero;

            mask = maskObject.AddComponent<RawImage>();
            mask.color = ColorUtils.TransparentColor;
            mask.raycastTarget = true;

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(OnMaskPointClick);

            EventTrigger eventTrigger = maskObject.AddComponent<EventTrigger>();
            eventTrigger.triggers.Add(entry);
        }

        private void OnMaskPointClick(BaseEventData eventData)
        {
            if (Modal) return;
            RemoveContainer();
        }

        public GameObject AddChild(GameObject prefab)
        {
            child = UnityUtils.AddChild(gameObject, prefab);
            child.name = "Prefab";
            return child;
        }

        public void RemoveContainer()
        {
            DestroyDelegate.Call(Path);
            Destroy(gameObject);
        }

        public void SetMaskAlpha(float alpha)
        {
            if (alpha <= 0f)
            {
                mask.color = ColorUtils.TransparentColor;
            }
            else
            {
                mask.color = new Color(0f, 0f, 0f, alpha);
            }
        }

        public void SetMaskBlur()
        {
            Camera[] cameras = new Camera[] { Camera.main };

            BlurUtils.BlurCameras(cameras);

            Texture2D texture2D = ScreenshotUtils.Screenshot(cameras, TextureFormat.RGB24);
            mask.color = Color.white;
            mask.texture = texture2D;

            BlurUtils.UnBlurCameras(cameras);
        }

        public void SetTop()
        {
            transform.SetAsLastSibling();
        }
    }
}
