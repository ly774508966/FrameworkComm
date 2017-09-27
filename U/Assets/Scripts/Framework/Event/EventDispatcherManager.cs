using System;

/// <summary>
/// 事件管理器
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public enum EventDispatcherId { MODEL, MSGID, CMDID };

    public class EventDispatcherManager : Singleton<EventDispatcherManager>
    {
        private EventDispatcher[] _dispatchers = new EventDispatcher[3];

        public EventDispatcherManager()
        {
            _dispatchers[0] = new EventDispatcher();
            _dispatchers[0].id = (uint)EventDispatcherId.MODEL;

            _dispatchers[1] = new EventDispatcher();
            _dispatchers[1].id = (uint)EventDispatcherId.MSGID;

            _dispatchers[2] = new EventDispatcher();
            _dispatchers[2].id = (uint)EventDispatcherId.CMDID;
        }

        EventDispatcher GetEventDispatcher(string sTypeName, ref int iEventType)
        {
            if (sTypeName.IndexOf("ModelEvent") != -1)
            {
                iEventType = (int)EventDispatcherId.MODEL;
                return _dispatchers[iEventType];
            }
            if (sTypeName.IndexOf("MSGID") != -1)
            {
                iEventType = (int)EventDispatcherId.MSGID;
                return _dispatchers[iEventType];
            }
            if (sTypeName.IndexOf("CMDID") != -1)
            {
                iEventType = (int)EventDispatcherId.CMDID;
                return _dispatchers[iEventType];
            }
            return null;
        }

        public void FireEvent<T>(T eventId, object param1 = null, object param2 = null)
        {
            int iEventType = 0;
            EventDispatcher evtDispatcher = GetEventDispatcher(typeof(T).FullName, ref iEventType);
            if (evtDispatcher != null)
            {
                evtDispatcher.FireEvent(Convert.ToUInt32(eventId), param1, param2);
            }
            else
            {
                Log.Error("FireEvent error type: " + typeof(T).FullName);
            }
        }

        public void AttachListener<T>(IEventListener listener, T eventId, bool bAttach = true)
        {
            int index = 0;
            EventDispatcher eventDispatcher = GetEventDispatcher(typeof(T).FullName, ref index);
            if (eventDispatcher != null)
            {
                eventDispatcher.AttachListener(listener, Convert.ToUInt32(eventId), bAttach);
            }
            else
            {
                Log.Error("AttachListener error type: " + typeof(T).FullName);
            }
        }
    }
}
