using System.Collections;

/// <summary>
/// 事件派发器
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public interface IEventListener
    {
        bool OnFireEvent(uint dispatcherId, uint key, object param1, object param2);
        int GetListenerPriority(uint eventKey);
    }

    public class EventDispatcher
    {
        private Hashtable _listenerHs = new Hashtable();
        private int _iDispatcherPriority = 0;
        private uint _iDispatcherId = 0;

        public int iDispatcherPriority
        {
            set { _iDispatcherPriority = value; }
            get { return _iDispatcherPriority; }
        }

        public uint id
        {
            set { _iDispatcherId = value; }
            get { return _iDispatcherId; }
        }

        void _AttachListener(IEventListener listener, uint eventKey)
        {
            if (listener == null)
            {
                Log.Error("EventDispacher.AttachListenerNow: failed due to no listener or event key specified.");
                return;
            }

            if (System.Threading.Thread.CurrentThread.ManagedThreadId != 1)
                Log.Debug("EventDispacher.AttachListenerNow: " + eventKey);
            if (!_listenerHs.ContainsKey(eventKey))
                _listenerHs.Add(eventKey, new ArrayList());

            ArrayList listenerList = _listenerHs[eventKey] as ArrayList;
            if (listenerList.Contains(listener))
            {
                Log.Error("EventDispacher.AttachListenerNow: " + listener.GetType().ToString() + " is already in list for event: " + eventKey.ToString());
                return;
            }

            int pos = 0;
            for (int n = 0; n < listenerList.Count; n++)
            {
                if (listener.GetListenerPriority(eventKey) > (listenerList[n] as IEventListener).GetListenerPriority(eventKey))
                {
                    break;
                }
                pos++;
            }
            listenerList.Insert(pos, listener);
        }

        void _DetachListener(IEventListener listener, uint eventKey)
        {
            if (listener == null)
            {
                Log.Error("EventDispacher.DetachListenerNow: failed due to no listener or event key specified.");
                return;
            }

            if (System.Threading.Thread.CurrentThread.ManagedThreadId != 1)
                Log.Debug("EventDispacher.DetachListenerNow: " + eventKey);
            if (!_listenerHs.ContainsKey(eventKey))
                return;

            ArrayList listenerList = _listenerHs[eventKey] as ArrayList;
            if (!listenerList.Contains(listener))
                return;

            listenerList.Remove(listener);
        }

        public void AttachListener(IEventListener listener, uint eventKey, bool bIsAttach)
        {
            if (bIsAttach)
            {
                _AttachListener(listener, eventKey);
            }
            else
            {
                _DetachListener(listener, eventKey);
            }
        }

        public bool FireEvent(uint key, object param1 = null, object param2 = null)
        {
            if (!_listenerHs.ContainsKey(key))
            {
                return false;
            }

            ArrayList listenerList = _listenerHs[key] as ArrayList;
            for (int n = 0; n < listenerList.Count; n++)
            {
                if ((listenerList[n] as IEventListener).OnFireEvent(_iDispatcherId, key, param1, param2))
                {
                    return true;
                }
            }

            return false;
        }

        public void Clear()
        {
            _listenerHs.Clear();
        }
    }
}
