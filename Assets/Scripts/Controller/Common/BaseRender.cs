using Framework;

namespace Project
{
    public abstract class BaseRender : FBaseController
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