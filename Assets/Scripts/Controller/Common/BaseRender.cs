using UnityEngine;
using System.Collections;

namespace Framework
{
    public abstract class BaseRender : BaseController
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