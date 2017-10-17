﻿using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Scroll page with previous and next-button
/// @zhenhaiwang
/// </summary>
namespace Framework.UI
{
    public class UIScrollPNPage : UIScrollPage
    {
        public Button previous;
        public Button next;

        protected override void InitView()
        {
            base.InitView();
            previous.onClick.AddListener(OnClickPrevious);
            next.onClick.AddListener(OnClickNext);
        }

        private void OnClickPrevious()
        {
            if (currentPageIndex > 0)
            {
                ScrollToPage(currentPageIndex - 1);
            }
        }

        private void OnClickNext()
        {
            if (currentPageIndex < currentPageCount - 1)
            {
                ScrollToPage(currentPageIndex + 1);
            }
        }

        /// <summary>
        /// jump to target page immediately, and update PN button
        /// </summary>
        public override void SetPage(int index)
        {
            if (index >= 0 && index < currentPageCount)
            {
                base.SetPage(index);
                UpdatePNButton();
            }
        }

        /// <summary>
        /// scroll to target page, and update PN button
        /// </summary>
        public override void ScrollToPage(int index)
        {
            if (index >= 0 && index < currentPageCount)
            {
                base.ScrollToPage(index);
                UpdatePNButton();
            }
        }

        /// <summary>
        /// scroll to target page after delay time, and update PN button
        /// </summary>
        public override void ScrollToPage(int index, float delay, Action callback = null)
        {
            if (index >= 0 && index < currentPageCount)
            {
                if (delay > 0)
                {
                    iTweenUtils.CreateTimeout(gameObject, delegate ()
                    {
                        ScrollToPage(index);
                        callback.Call();
                    }, delay, "UIScrollPNPage_ScrollToPage");
                }
                else
                {
                    ScrollToPage(index);
                }
            }
        }

        public void UpdatePNButton()
        {
            previous.gameObject.SetActive(currentPageIndex > 0);
            next.gameObject.SetActive(currentPageIndex < currentPageCount - 1);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            UpdatePNButton();
        }
    }
}