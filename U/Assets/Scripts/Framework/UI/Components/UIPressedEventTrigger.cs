using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework.UI
{
    [Serializable]
    internal class PressedEvent : UnityEvent<bool, string> { }

    public class UIPressedEventTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [Range(0.1f, 1.0f)]
        public float delay = 0.25f;

        [SerializeField]
        PressedEvent m_OnPressed = new PressedEvent();

        public string parameter = "";

        bool m_bPressed = false;
        bool m_bPressedInvoked = false;
        float m_fPressedTime = 0.0f;

        void Update()
        {
            if (m_bPressed && Time.time - m_fPressedTime > delay)
            {
                m_bPressed = false;
                m_bPressedInvoked = true;
                m_OnPressed.Invoke(true, parameter);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            m_fPressedTime = Time.time;
            m_bPressed = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            m_bPressed = false;

            if (m_bPressedInvoked)
            {
                m_bPressedInvoked = false;
                m_OnPressed.Invoke(false, parameter);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_bPressed = false;

            if (m_bPressedInvoked)
            {
                m_bPressedInvoked = false;
                m_OnPressed.Invoke(false, parameter);
            }
        }
    }
}