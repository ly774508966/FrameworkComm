using UnityEngine;

/// <summary>
/// MonoBehaviour单例基类
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        protected static T _instance = null;
        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameSystem.instance.GetManager<T>();
                }
                return _instance;
            }
        }

        protected virtual void OnDestroy()
        {
            _instance = null;
        }
    }
}