namespace Framework
{
    public abstract class FBaseModel<T> : FSingleton<T> where T : class, new()
    {
        protected bool _ready = false;
        public bool ready
        {
            get { return _ready; }
        }

        public bool enabled = true;

        protected FBaseModel()
        {
            InitData();
        }

        protected virtual void InitData()
        {
            _ready = true;
        }
    }
}