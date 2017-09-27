using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Presenter基类，提供访问逻辑数据层的能力，并增加逻辑层的协程处理和自动回收机制
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public class BasePresenter : MonoBehaviour, IEventListener
    {
        protected delegate bool EventHandler(object param1, object param2);
        private Dictionary<ModelEvent, EventHandler> _eventDic = null;

        void Start()
        {
            InitEvent(true);
            InitPresenter();
        }

        /// <summary>
        /// 子类重写后不要忘记base.OnDestroy
        /// </summary>
        protected virtual void OnDestroy()
        {
            InitEvent(false);
        }

        /// <summary>
        /// 事件监听和事件响应函数
        /// </summary>
        protected virtual void InitEvent(Dictionary<ModelEvent, EventHandler> eventDic) { }

        protected virtual void InitPresenter() { }

        public virtual bool OnFireEvent(uint dispatcherId, uint key, object param1, object param2)
        {
            EventHandler handlers = _eventDic[(ModelEvent)key];
            bool bRet = false;
            if (handlers != null)
                bRet = handlers(param1, param2);
            return bRet;
        }

        public virtual int GetListenerPriority(uint eventKey)
        {
            return 0;
        }

        void InitEvent(bool bAttach)
        {
            if (_eventDic == null)
            {
                _eventDic = new Dictionary<ModelEvent, EventHandler>();
                InitEvent(_eventDic);
            }

            foreach (ModelEvent key in _eventDic.Keys)
            {
                EventDispatcherManager.instance.AttachListener(this, key, bAttach);
            }
        }
    }
}
