/// <summary>
/// 普通单例基类
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public abstract class Singleton<T> where T : class, new()
    {
        protected static T _instance = null;
        private static readonly object _syslock = new object();

        public static T instance
        {
            get
            {
                lock (_syslock)
                {
                    if (_instance == null)
                    {
                        new T();
                    }
                }

                return _instance;
            }
        }

        public bool enabled { get; set; }

        protected Singleton()
        {
            _instance = this as T;

            enabled = true;
            OnInit();
        }

        ~Singleton()
        {
            enabled = false;
            OnDestroy();
        }

        protected virtual void OnInit() { }

        protected virtual void OnDestroy() { }

        protected void DestroyInstance()
        {
            _instance = null;
        }
    }
}