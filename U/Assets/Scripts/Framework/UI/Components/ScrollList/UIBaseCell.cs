/// <summary>
/// UICell基类
/// @zhenhaiwang
/// </summary>
namespace Framework.UI
{
    public class UIBaseCell : BaseView
    {
        public int index { get; set; }

        private object _data = null;
        public virtual object data
        {
            get { return _data; }
            set
            {
                _data = value;

                if (_data == null)
                {
                    OnRecycle();
                }
                else
                {
                    InvalidView();
                }
            }
        }

        protected virtual void OnRecycle() { }
    }
}
