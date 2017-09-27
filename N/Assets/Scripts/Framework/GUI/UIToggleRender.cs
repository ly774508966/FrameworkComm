using UnityEngine;
using System.Collections;

namespace Framework
{
    public class UIToggleRender : FBaseRender
    {
        private UIToggleList _owner = null;
        public UIToggleList owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        protected virtual void FireEvent(string name, object param = null)
        {
            _owner.TriggerRenderEvent(name, param == null ? data : param);
        }

        protected override void ClearData() { base.ClearData(); _owner = null; }
    }
}