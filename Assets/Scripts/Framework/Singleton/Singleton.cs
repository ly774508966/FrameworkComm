using UnityEngine;
using System;
using System.Reflection;

namespace Framework
{
    /// <summary>
    /// 单例基类
    /// </summary>
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
                        _instance = new T();
                    }
                }

                return _instance;
            }
        }

        protected Singleton() { }

        protected void Dispose()
        {
            _instance = null;
        }
    }
}