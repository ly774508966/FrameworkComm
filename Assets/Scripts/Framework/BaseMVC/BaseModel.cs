using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework
{
    public abstract class BaseModel<T> : Singleton<T> where T : class, new()
    {
        protected bool _ready = false;
        public bool ready
        {
            get { return _ready; }
        }

        public bool enabled = true;

        protected BaseModel()
        {
            InitData();
        }

        protected virtual void InitData()
        {
            _ready = true;
        }
    }
}