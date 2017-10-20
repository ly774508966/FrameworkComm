/// <summary>
/// UICell基类
/// @zhenhaiwang
/// </summary>
namespace Framework.UI
{
    public class UIBaseCell : BaseView
    {
        private int _index;
        public int index
        {
            get { return _index;  }
            set
            {
                _index = value;  
                InvalidView();
            }
        }

        private object _data = null;
        public virtual object data
        {
            get { return _data; }
            set
            {
                _data = value;

                if (_data == null)
                {
                    UpdateView();
                }
                else
                {
                    InvalidView();
                }
            }
        } 
    }
}
