using System;

namespace Framework
{
    /// <summary>
    /// TabPage类型Render组件基类
    /// by zhenhaiwang
    /// </summary>
    public abstract class TabPageRender : FBaseRender
    {
        private TabPageList _owner = null;
        public TabPageList owner
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

