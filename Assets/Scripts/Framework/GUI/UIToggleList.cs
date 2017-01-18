using UnityEngine;
using System;

namespace Framework
{
    public class UIToggleList : FBaseController
    {
        public delegate void RenderEvent(string name, object data);
        public event RenderEvent renderEvent;

        public UITable table;
        public GameObject prefab;

        private int _groupId = 1;
        private FRenderCache<UIToggle> _toggleCache;

        private object[] _data;
        public object[] data
        {
            set { _data = value; InvalidView(); }
        }

        protected override void InitUI()
        {
            base.InitUI();

            _toggleCache = new FRenderCache<UIToggle>(table, prefab);
        }

        protected override void UpdateView()
        {
            base.UpdateView();

            int length = _data != null ? _data.Length : 0;
            if (length <= 0)
            {
                ClearData();
                return;
            }

            for (int i = 0; i < length; i++)
            {
                UIToggle toggle;

                toggle = _toggleCache.PushRender(i);

                toggle.group = _groupId;
                EventDelegate.Add(toggle.onChange, TabOnChange);
                if (i == 0) toggle.startsActive = true;

                UIToggleRender tRender = toggle.GetComponent<UIToggleRender>();
                if (tRender != null)
                {
                    tRender.data = _data[i];
                    tRender.owner = this;
                }
            }

            _toggleCache.Release();
            table.Reposition();
        }

        void TabOnChange()
        {
            UIToggle toggle = UIToggle.current;
            if (toggle.value)
            {
                UIToggleRender tRender = toggle.GetComponent<UIToggleRender>();
                if (tRender != null) TriggerRenderEvent("OnChange", tRender.data);
            }
        }

        public virtual void TriggerRenderEvent(string name, object data)
        {
            if (renderEvent != null) renderEvent(name, data);
        }

        void ClearData()
        {
            data = null;
            if (_toggleCache != null)
            {
                _toggleCache.Clear();
                _toggleCache = null;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ClearData();
        }
    }
}