using UnityEngine;
using System.Collections;

namespace Framework
{
    /// <summary>
    /// Event Listener interface
    /// </summary>
    public interface IEventListener
    {
        /// <summary>
        /// 事件派发
        /// </summary>
        /// <returns> 拦截事件 return true，继续派发事件 return false </returns>
        bool OnFireEvent(uint dispatcherId, uint key, object param1, object param2);

        /// <summary>
        /// priority of this listener to get event with specified eventKey
        /// the larger the higher
        /// set before attach
        /// arrange sequence only happens a single time at Attach
        /// </summary>
        int GetListenerPriority(uint eventKey);
    }

    /// <summary>
    /// 事件派发器基类
    /// 1. 派发器拥有优先级，优先级高的派发器会优先派发事件
    /// 2. 如果事件被监听者拦截，则之后的低优先级派发器不再派发
    /// </summary>
    public class FEventDispatcher
    {
        private Hashtable _listenerHashTable = new Hashtable();
        private int _dispatcherPriority = 0;
        private uint _dispatcherId = 0;

        public int dispatcherPriority
        {
            set { _dispatcherPriority = value; }
            get { return _dispatcherPriority; }
        }

        public uint id
        {
            set { _dispatcherId = value; }
            get { return _dispatcherId; }
        }

        private void AttachListner(uint eventKey, ArrayList listenerList, IEventListener listener)
        {
            int pos = 0;
            for (int n = 0; n < listenerList.Count; n++)
            {
                // the one last added gains the least priority 
                if (listener.GetListenerPriority(eventKey) > (listenerList[n] as IEventListener).GetListenerPriority(eventKey))
                {
                    break;
                }
                pos++;
            }
            listenerList.Insert(pos, listener);
        }

        /// <summary>
        /// 非线程安全，只允许在主线程调用
        /// </summary>
        public void AttachListenerNow(IEventListener listener, uint eventKey)
        {
            if (listener == null)
            {
                FLog.Error("AttachListenerNow failed due to no listener or event key specified in EventDispacher.");
                return;
            }

            if (System.Threading.Thread.CurrentThread.ManagedThreadId != 1)
                FLog.Debug("AttachListenerNow -> " + eventKey);
            if (!_listenerHashTable.ContainsKey(eventKey))
                _listenerHashTable.Add(eventKey, new ArrayList());

            ArrayList listenerList = _listenerHashTable[eventKey] as ArrayList;
            if (listenerList.Contains(listener))
            {
                FLog.Error("AttachListenerNow -> " + listener.GetType().ToString() + " is already in list for event: " + eventKey.ToString() + " in EventDispacher.");
                return;
            }

            AttachListner(eventKey, listenerList, listener);
        }

        /// <summary>
        /// 非线程安全,只允许在主线程调用
        /// </summary>
        public void DetachListenerNow(IEventListener listener, uint eventKey)
        {
            if (listener == null)
            {
                FLog.Error("DetachListenerNow failed due to no listener or event key specified in EventDispacher.");
                return;
            }

            if (System.Threading.Thread.CurrentThread.ManagedThreadId != 1)
                FLog.Debug("DetachListenerNow -> " + eventKey);
            if (!_listenerHashTable.ContainsKey(eventKey))
                return;

            ArrayList listenerList = _listenerHashTable[eventKey] as ArrayList;
            if (!listenerList.Contains(listener))
                return;

            listenerList.Remove(listener);
        }

        public void AttachListenerNow(IEventListener listener, uint eventKey, bool bIsAttach)
        {
            if (bIsAttach)
            {
                AttachListenerNow(listener, eventKey);
            }
            else
            {
                DetachListenerNow(listener, eventKey);
            }
        }

        /// <summary>
        /// 立即派发事件
        /// </summary>
        public bool FireEvent(uint key, object param1 = null, object param2 = null)
        {
            if (!_listenerHashTable.ContainsKey(key))
            {
                return false;
            }

            ArrayList listenerList = _listenerHashTable[key] as ArrayList;
            for (int n = 0; n < listenerList.Count; n++)
            {
                if ((listenerList[n] as IEventListener).OnFireEvent(_dispatcherId, key, param1, param2))
                {
                    return true;
                }
            }

            return false;
        }

        public void Clear()
        {
            FLog.Debug("ListenerHashTable.Clear() in FEventDispatcher.");
            _listenerHashTable.Clear();
        }
    }
}
