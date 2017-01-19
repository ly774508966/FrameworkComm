namespace Framework
{
    /// <summary>
    /// 所有 List Render 的基类
    /// by zhenhaiwang
    /// </summary>
    public abstract class FBaseRender : FBaseController
    {
        private object _data = null;
        public object data
        {
            get { return _data; }
            set
            {
                _data = value;
                InvalidView();
            }
        }

        protected virtual void ClearData() { data = null; }
    }
}