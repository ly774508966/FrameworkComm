using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework
{
    public abstract class BaseModel<T> : Singleton<T> where T : class, new()
    {
        protected bool _initOver = false;
        public bool IsInitialized
        {
            get { return _initOver; }
        }

        public BaseModel()
        {
            InitData();
        }

        protected virtual void InitData()
        {
            _initOver = true;
        }
    }
}